/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2015 dotNetRDF Project (dotnetrdf-develop@lists.sf.net)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using VDS.RDF.Graphs.Utilities;
using System.Linq;
using VDS.RDF.Collections;
using VDS.RDF.Namespaces;
using VDS.RDF.Nodes;

namespace VDS.RDF.Graphs
{
    /// <summary>
    /// Abstract Base Implementation of the <see cref="IGraph">IGraph</see> interface
    /// </summary>
    public abstract class BaseGraph 
        : NodeFactory, IEventedGraph
    {

        /// <summary>
        /// Collection of Triples in the Graph
        /// </summary>
        protected ITripleCollection _triples;
        /// <summary>
        /// Namespace Mapper
        /// </summary>
        protected readonly NamespaceMapper _nsmapper;
        private readonly NotifyCollectionChangedEventHandler _changedHandler;

        /// <summary>
        /// Creates a new Base Graph using the given Triple Collection
        /// </summary>
        /// <param name="tripleCollection">Triple Collection to use</param>
        protected BaseGraph(ITripleCollection tripleCollection)
        {
            this._triples = tripleCollection;
            this._nsmapper = new NamespaceMapper();
            this._changedHandler = this.HandleTripleCollectionChanged;

            // Attach event handlers
            this.AttachEventHandlers();
        }

        /// <summary>
        /// Creates a new Base Graph which uses the default <see cref="TreeIndexedTripleCollection" /> as the Triple Collection
        /// </summary>
        protected BaseGraph()
            : this(new TreeIndexedTripleCollection()) { }


        /// <summary>
        /// Gets the set of Triples in this Graph
        /// </summary>
        public virtual IEnumerable<Triple> Triples
        {
            get
            {
                return this._triples;
            }
        }

        /// <summary>
        /// Gets the set of Quads in the graph
        /// </summary>
        /// <remarks>
        /// Since a graph has no name directly associated with it the resulting quads will have the null name assigned to them and so will appears as if in the default unnamed graph
        /// </remarks>
        public virtual IEnumerable<Quad> Quads
        {
            get
            {
                return this._triples.Select(t => t.AsQuad(null));
            }
        }

        /// <summary>
        /// Gets the nodes that are used as vertices in the graph i.e. those which occur in the subject or object position of a triple
        /// </summary>
        public virtual IEnumerable<INode> Vertices
        {
            get
            {
                return (from t in this._triples
                        select t.Subject).Concat(from t in this._triples
                                                 select t.Object).Distinct();
            }
        }

        /// <summary>
        /// Gets the nodes that are used as edges in the graph i.e. those which occur in the predicate position of a triple
        /// </summary>
        public virtual IEnumerable<INode> Edges
        {
            get
            {
                return (from t in this._triples
                        select t.Predicate).Distinct();
            }
        }

        /// <summary>
        /// Gets the Namespace Mapper for this Graph which contains all in use Namespace Prefixes and their URIs
        /// </summary>
        /// <returns></returns>
        public virtual INamespaceMapper Namespaces
        {
            get
            {
                return this._nsmapper;
            }
        }

        /// <summary>
        /// Gets the number of triples in the graph
        /// </summary>
        public virtual long Count
        {
            get
            {
                return this._triples.Count;
            }
        }

        /// <summary>
        /// Gets whether a Graph is Empty ie. Contains No Triples or Nodes
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                return (this._triples.Count == 0);
            }
        }

        /// <summary>
        /// Gets the capabilities of the graph
        /// </summary>
        public virtual IGraphCapabilities Capabilities
        {
            get
            {
                return new TripleCollectionCapabilities(this._triples, this._triples.IsReadOnly ? GraphAccessMode.Read : GraphAccessMode.ReadWrite);
            }
        }

        /// <summary>
        /// Asserts a Triple in the Graph
        /// </summary>
        /// <param name="t">The Triple to add to the Graph</param>
        public abstract void Assert(Triple t);

        /// <summary>
        /// Asserts a List of Triples in the graph
        /// </summary>
        /// <param name="ts">List of Triples in the form of an IEnumerable</param>
        public abstract void Assert(IEnumerable<Triple> ts);

        /// <summary>
        /// Retracts a Triple from the Graph
        /// </summary>
        /// <param name="t">Triple to Retract</param>
        /// <remarks>Current implementation may have some defunct Nodes left in the Graph as only the Triple is retracted</remarks>
        public abstract void Retract(Triple t);

        /// <summary>
        /// Retracts a enumeration of Triples from the graph
        /// </summary>
        /// <param name="ts">Enumeration of Triples to retract</param>
        public abstract void Retract(IEnumerable<Triple> ts);

        /// <summary>
        /// Clears all Triples from the Graph
        /// </summary>
        public virtual void Clear()
        {
            this._triples.Clear();
        }

        /// <summary>
        /// Creates a new URI Node with the given prefixed name
        /// </summary>
        /// <param name="prefixedName">Prefixed name for the Node</param>
        /// <returns>URI Node</returns>
        /// <remarks>Internally the Graph will resolve the prefixed name to a full URI, this throws an exception when this is not possible</remarks>
        public virtual INode CreateUriNode(String prefixedName)
        {
            return new UriNode(UriFactory.ResolvePrefixedName(prefixedName, this._nsmapper, null));
        }

        /// <summary>
        /// Finds triples matching the given search criteria i.e. those where the given nodes occur in the appropriate position(s).  Null values are considered wildcards for a position.
        /// </summary>
        /// <param name="s">Subject</param>
        /// <param name="p">Predicate</param>
        /// <param name="o">Object</param>
        /// <returns>Triples</returns>
        public virtual IEnumerable<Triple> Find(INode s, INode p, INode o)
        {
            if (ReferenceEquals(s, null))
            {
                // Wildcard Subject
                if (ReferenceEquals(p, null))
                {
                    // Wildcard Subject and Predicate
                    if (ReferenceEquals(o, null))
                    {
                        // Wildcard Subject, Predicate and Object
                        return this.Triples;
                    }
                    // Wildcard Subject and Predicate with Fixed Object
                    return this._triples.WithObject(o);
                }
                // Fixed Predicate with Wildcard Subject
                if (ReferenceEquals(o, null))
                {
                    // Fixed Predicate with Wildcard Subject and Object
                    return this._triples.WithPredicate(p);
                }
                // Fixed Predicate and Object with Wildcard Subject
                return this._triples.WithPredicateObject(p, o);
            }

            // Fixed Subject
            if (ReferenceEquals(p, null))
            {
                // Wildcard Predicate with Fixed Subject
                if (ReferenceEquals(o, null))
                {
                    // Wildcard Predicate and Object with Fixed Subject
                    return this._triples.WithSubject(s);
                }
                // Wildcard Predicate with Fixed Subject and Object
                return this._triples.WithSubjectObject(s, o);
            }

            // Fixed Subject and Predicate
            if (ReferenceEquals(o, null))
            {
                // Fixed Subject and Predicate with Wildcard Object
                return this._triples.WithSubjectPredicate(s, p);
            }

            // Fixed Subject, Predicate and Object
            Triple t = new Triple(s, p, o);
            return this._triples.Contains(t) ? t.AsEnumerable() : Enumerable.Empty<Triple>();
        }

        /// <summary>
        /// Gets whether a given Triple exists in this Graph
        /// </summary>
        /// <param name="t">Triple to test</param>
        /// <returns></returns>
        public virtual bool ContainsTriple(Triple t)
        {
            return this._triples.Contains(t);
        }

        /// <summary>
        /// Determines whether a Graph is equal to another Object
        /// </summary>
        /// <param name="obj">Object to test</param>
        /// <returns></returns>
        /// <remarks>
        /// Graphs are only considered equal if they have the same reference, to check if two graphs are equivalent use the <see cref="GraphExtensions.IsIsomorphicWith(IGraph,IGraph)"/> method
        /// </remarks>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        /// <summary>
        /// Gets whether the graph has events
        /// </summary>
        public virtual bool HasEvents { get { return true; } }

        /// <summary>
        /// Attachs the event handles to the underying <see cref="ITripleCollection"/>
        /// </summary>
        protected void AttachEventHandlers()
        {
            this._triples.CollectionChanged += this._changedHandler;
        }

        /// <summary>
        /// Helper method used to catch the <see cref="INotifyCollectionChanged.CollectionChanged"/> from the underlying <see cref="ITripleCollection"/> and propogate the event up through this graphs <see cref="CollectionChanged"/> event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Arguments</param>
        private void HandleTripleCollectionChanged(Object sender, NotifyCollectionChangedEventArgs args)
        {
            this.RaiseCollectionChanged(args);
        }

        /// <summary>
        /// Raises the collection changed event
        /// </summary>
        /// <param name="args">Collection changed arguments</param>
        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler d = this.CollectionChanged;
            if (d != null)
            {
                d(this, args);
            }
        }

        /// <summary>
        /// Events which is raised when the graph changes
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Disposes of a Graph
        /// </summary>
        public virtual void Dispose()
        {
            // Nothing to do by default
            // We don't do anything to the triple collection because it could be bound to multiple graphs
        }

    }
}
