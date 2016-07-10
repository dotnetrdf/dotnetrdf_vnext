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
using System.Linq;
using Xunit;
using VDS.RDF.Nodes;

namespace VDS.RDF.Graphs
{
    /// <summary>
    /// Abstract tests for <see cref="IGraph"/> implementations
    /// </summary>
    public abstract class AbstractGraphContractTests
        : AbstractNodeFactoryContractTests
    {
        protected override sealed INodeFactory CreateFactoryInstance()
        {
            return (INodeFactory) this.CreateGraphInstance();
        }

        /// <summary>
        /// Gets a new fresh instance of a graph for testing
        /// </summary>
        /// <returns></returns>
        protected abstract IGraph CreateGraphInstance();


        protected bool ProvidesAccess(IGraph g, GraphAccessMode mode)
        {
            return g.Capabilities.AccessMode >= mode;
        }

        protected IEnumerable<Triple> GenerateTriples(int n)
        {
            for (int i = 0; i < n; i++)
            {
                yield return new Triple(new UriNode(new Uri("http://test/" + i)), new UriNode(new Uri("http://predicate")), new LiteralNode(i.ToString()));
            }
        }

        [Fact]
        public void GraphContractCount1()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.Read)) return;

            Assert.Equal(0, g.Count);
        }

        [Fact]
        public void GraphContractCount2()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            g.Assert(this.GenerateTriples(1));
            Assert.Equal(1, g.Count);
        }

        [Fact]
        public void GraphContractCount3()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            g.Assert(this.GenerateTriples(100));
            Assert.Equal(100, g.Count);
        }

        [Fact]
        public void GraphContractIsEmpty1()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.Read)) return;

            Assert.True(g.IsEmpty);
        }

        [Fact]
        public void GraphContractIsEmpty2()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            g.Assert(this.GenerateTriples(1));
            Assert.False(g.IsEmpty);
        }

        [Fact]
        public void GraphContractNamespaces1()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.Read)) return;

            Assert.NotNull(g.Namespaces);
        }

        [Fact]
        public void GraphContractNamespaces2()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.NotNull(g.Namespaces);
            g.Namespaces.AddNamespace("ex", new Uri("http://example.org"));
            Assert.True(g.Namespaces.HasNamespace("ex"));
            Assert.Equal(new Uri("http://example.org"), g.Namespaces.GetNamespaceUri("ex"));
        }

        [Fact]
        public void GraphContractAssert1()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.False(g.Triples.Any());

            // Assert the triple
            Triple t = new Triple(g.CreateUriNode(new Uri("http://subject")), g.CreateUriNode(new Uri("http://predicate")), g.CreateBlankNode());
            g.Assert(t);
            Assert.Equal(1, g.Count);
            Assert.True(g.ContainsTriple(t));
            Assert.True(g.Triples.Any());

            // Asserting same triple again should have no effect
            g.Assert(t);
            Assert.Equal(1, g.Count);
            Assert.True(g.ContainsTriple(t));
            Assert.True(g.Triples.Any());
        }

        [Fact]
        public void GraphContractAssert2()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.False(g.Triples.Any());

            // Assert triples
            List<Triple> ts = new List<Triple>();
            for (int i = 0; i < 10000; i++)
            {
                ts.Add(new Triple(g.CreateUriNode(new Uri("http://subject")), g.CreateUriNode(new Uri("http://predicate")), i.ToLiteral(g)));
            }
            g.Assert(ts);
            Assert.Equal(ts.Count, g.Count);
            foreach (Triple t in ts)
            {
                Assert.True(g.ContainsTriple(t));
            }

            // Asserting same triples again should have no effect
            g.Assert(ts);
            Assert.Equal(ts.Count, g.Count);
            foreach (Triple t in ts)
            {
                Assert.True(g.ContainsTriple(t));
            }
        }

        [Fact]
        public void GraphContractRetract1()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.False(g.Triples.Any());

            // Assert the triple
            Triple t = new Triple(g.CreateUriNode(new Uri("http://subject")), g.CreateUriNode(new Uri("http://predicate")), g.CreateBlankNode());
            g.Assert(t);
            Assert.Equal(1, g.Count);
            Assert.True(g.ContainsTriple(t));
            Assert.True(g.Triples.Any());

            // Retract the triple
            g.Retract(t);
            Assert.Equal(0, g.Count);
            Assert.False(g.ContainsTriple(t));
            Assert.False(g.Triples.Any());
        }

        [Fact]
        public void GraphContractRetract2()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.False(g.Triples.Any());

            // Assert triples
            List<Triple> ts = new List<Triple>();
            for (int i = 0; i < 10000; i++)
            {
                ts.Add(new Triple(g.CreateUriNode(new Uri("http://subject")), g.CreateUriNode(new Uri("http://predicate")), i.ToLiteral(g)));
            }
            g.Assert(ts);
            Assert.Equal(ts.Count, g.Count);
            foreach (Triple t in ts)
            {
                Assert.True(g.ContainsTriple(t));
            }

            // Retract triples
            g.Retract(ts);
            Assert.Equal(0, g.Count);
            foreach (Triple t in ts)
            {
                Assert.False(g.ContainsTriple(t));
            }
        }

        [Fact]
        public void GraphContractTriples1()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.False(g.Triples.Any());

            // Assert the triple
            Triple t = new Triple(g.CreateUriNode(new Uri("http://subject")), g.CreateUriNode(new Uri("http://predicate")), g.CreateBlankNode());
            g.Assert(t);
            Assert.Equal(1, g.Count);
            Assert.True(g.ContainsTriple(t));
            Assert.True(g.Triples.Any());

            IEnumerable<Triple> ts = g.Triples;
            Assert.True(ts.Any());
            Assert.Equal(1, ts.Count());
            Assert.True(ts.Contains(t));

            // Retract the triple
            g.Retract(t);
            Assert.Equal(0, g.Count);
            Assert.False(g.ContainsTriple(t));
            Assert.False(g.Triples.Any());

            // Enumerable should reflect current state of graph
            Assert.False(ts.Any());
            Assert.Equal(0, ts.Count());
            Assert.False(ts.Contains(t));
        }

        [Fact]
        public void GraphContractTriples2()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.False(g.Triples.Any());

            // Asserts
            List<Triple> data = new List<Triple>();
            for (int i = 0; i < 1000; i++)
            {
                data.Add(new Triple(g.CreateUriNode(new Uri("http://subject")), g.CreateUriNode(new Uri("http://predicate")), i.ToLiteral(g)));
            }
            g.Assert(data);
            Assert.Equal(data.Count, g.Count);
            foreach (Triple t in data)
            {
                Assert.True(g.ContainsTriple(t));
                Assert.True(g.Triples.Any());
            }

            IEnumerable<Triple> ts = g.Triples;
            Assert.True(ts.Any());
            Assert.Equal(data.Count, ts.Count());
            foreach (Triple t in ts)
            {
                Assert.True(data.Remove(t));
            }

            // Retract triples
            g.Retract(ts.ToList());
            Assert.Equal(0, g.Count);
            foreach (Triple t in ts)
            {
                Assert.False(g.ContainsTriple(t));
            }

            // Enumerable should reflect current state of graph
            Assert.False(ts.Any());
            Assert.Equal(0, ts.Count());
        }

        [Fact]
        public void GraphContractTriples3()
        {
            IGraph g = this.CreateGraphInstance();
            if (!g.Capabilities.CanModifyDuringIteration)
            {
                // Graph does not have the capability to run this test
                return;
            }
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.False(g.Triples.Any());

            // Asserts
            List<Triple> data = new List<Triple>();
            for (int i = 0; i < 10; i++)
            {
                data.Add(new Triple(g.CreateUriNode(new Uri("http://subject")), g.CreateUriNode(new Uri("http://predicate")), i.ToLiteral(g)));
            }
            g.Assert(data);
            Assert.Equal(data.Count, g.Count);
            foreach (Triple t in data)
            {
                Assert.True(g.ContainsTriple(t));
                Assert.True(g.Triples.Any());
            }

            IEnumerable<Triple> ts = g.Triples;
            Assert.True(ts.Any());
            Assert.Equal(data.Count, ts.Count());
            foreach (Triple t in ts)
            {
                // Expect this to throw an error
                g.Retract(t);
            }
        }

        [Fact]
        public void GraphContractQuads1()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.False(g.Quads.Any());

            // Assert the triple
            Triple t = new Triple(g.CreateUriNode(new Uri("http://subject")), g.CreateUriNode(new Uri("http://predicate")), g.CreateBlankNode());
            g.Assert(t);
            Assert.Equal(1, g.Count);
            Assert.True(g.ContainsTriple(t));
            Assert.True(g.Triples.Any());

            IEnumerable<Quad> qs = g.Quads;
            Assert.True(qs.Any());
            Assert.Equal(1, qs.Count());
            Assert.True(qs.Contains(t.AsQuad(Quad.DefaultGraphNode)));

            // Retract the triple
            g.Retract(t);
            Assert.Equal(0, g.Count);
            Assert.False(g.ContainsTriple(t));
            Assert.False(g.Triples.Any());

            // Enumerable should reflect current state of graph
            Assert.False(qs.Any());
            Assert.Equal(0, qs.Count());
            Assert.False(qs.Contains(t.AsQuad(Quad.DefaultGraphNode)));
        }

        /// <summary>
        /// Performs some basic checks on the Find() implementation of a given graph
        /// </summary>
        /// <param name="g">Graph</param>
        protected static void FindBasicChecks(IGraph g)
        {
            // Find everything
            List<Triple> ts = g.Find(null, null, null).ToList();
            Assert.Equal(g.Count, ts.Count);

            // Find nothing
            ts = g.Find(g.CreateUriNode(new Uri("http://nosuchthing")), null, null).ToList();
            Assert.Equal(0, ts.Count);

            // Find each specific triple
            foreach (Triple t in g.Triples)
            {
                ts = g.Find(t.Subject, t.Predicate, t.Object).ToList();
                Assert.Equal(1, ts.Count);
            }
        }

        [Fact]
        public void GraphContractFind1()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.True(g.IsEmpty);

            INode s1 = g.CreateUriNode(new Uri("http://s1"));
            INode s2 = g.CreateUriNode(new Uri("http://s2"));
            INode p = g.CreateUriNode(new Uri("http://p"));
            INode o1 = g.CreateLiteralNode("value");
            INode o2 = g.CreateUriNode(new Uri("http://o"));

            Triple t1 = new Triple(s1, p, o1);
            g.Assert(t1);
            Triple t2 = new Triple(s1, p, o2);
            g.Assert(t2);
            Triple t3 = new Triple(s2, p, o2);
            g.Assert(t3);
            Assert.Equal(3, g.Count);

            // Perform basic checks
            FindBasicChecks(g);

            // Find by subject
            List<Triple> ts = g.Find(s1, null, null).ToList();
            Assert.Equal(2, ts.Count);
            Assert.True(ts.Contains(t1));
            Assert.True(ts.Contains(t2));

            // Find by predicate
            ts = g.Find(null, p, null).ToList();
            Assert.Equal(3, ts.Count);

            // Find by object
            ts = g.Find(null, null, o1).ToList();
            Assert.Equal(1, ts.Count);
            Assert.True(ts.Contains(t1));

            // Find by subject and object
            ts = g.Find(s2, null, o2).ToList();
            Assert.Equal(1, ts.Count);
            Assert.True(ts.Contains(t3));
        }

        [Fact]
        public void GraphContractFind2()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.True(g.IsEmpty);

            INode s1 = g.CreateBlankNode();
            INode s2 = g.CreateBlankNode();
            INode p = g.CreateUriNode(new Uri("http://p"));
            INode o1 = g.CreateLiteralNode("value");
            INode o2 = g.CreateUriNode(new Uri("http://o"));

            Triple t1 = new Triple(s1, p, o1);
            g.Assert(t1);
            Triple t2 = new Triple(s1, p, o2);
            g.Assert(t2);
            Triple t3 = new Triple(s2, p, o2);
            g.Assert(t3);
            Assert.Equal(3, g.Count);

            // Perform basic checks
            FindBasicChecks(g);

            // Find by subject
            List<Triple> ts = g.Find(s1, null, null).ToList();
            Assert.Equal(2, ts.Count);
            Assert.True(ts.Contains(t1));
            Assert.True(ts.Contains(t2));

            // Find by predicate
            ts = g.Find(null, p, null).ToList();
            Assert.Equal(3, ts.Count);

            // Find by object
            ts = g.Find(null, null, o2).ToList();
            Assert.Equal(2, ts.Count);
            Assert.True(ts.Contains(t2));
            Assert.True(ts.Contains(t3));

            // Find by subject and object
            ts = g.Find(s2, null, o2).ToList();
            Assert.Equal(1, ts.Count);
            Assert.True(ts.Contains(t3));

            ts = g.Find(s1, null, o1).ToList();
            Assert.Equal(1, ts.Count);
            Assert.True(ts.Contains(t1));

            ts = g.Find(s1, null, o2).ToList();
            Assert.Equal(1, ts.Count);
            Assert.True(ts.Contains(t2));

            ts = g.Find(s2, null, o1).ToList();
            Assert.Equal(0, ts.Count);
        }

        [Fact]
        public void GraphContractFind3()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.True(g.IsEmpty);

            INode s = g.CreateBlankNode();
            INode s2 = g.CreateBlankNode();
            INode p1 = g.CreateUriNode(new Uri("http://p1"));
            INode p2 = g.CreateUriNode(new Uri("http://p2"));
            INode o = g.CreateLiteralNode("value");
            INode o2 = g.CreateUriNode(new Uri("http://o"));

            Triple t1 = new Triple(s, p1, o);
            g.Assert(t1);
            Triple t2 = new Triple(s2, p1, o);
            g.Assert(t2);
            Triple t3 = new Triple(s, p2, o2);
            g.Assert(t3);
            Assert.Equal(3, g.Count);

            // Perform basic checks
            FindBasicChecks(g);

            // Find by subject
            List<Triple> ts = g.Find(s, null, null).ToList();
            Assert.Equal(2, ts.Count);
            Assert.True(ts.Contains(t1));
            Assert.True(ts.Contains(t3));

            // Find by predicate
            ts = g.Find(null, p1, null).ToList();
            Assert.Equal(2, ts.Count);

            // Find by predicate and object
            ts = g.Find(null, p1, o).ToList();
            Assert.Equal(2, ts.Count);
            Assert.True(ts.Contains(t1));
            Assert.True(ts.Contains(t2));

            ts = g.Find(null, p2, o2).ToList();
            Assert.Equal(1, ts.Count);
            Assert.True(ts.Contains(t3));

            ts = g.Find(null, p2, o).ToList();
            Assert.Equal(0, ts.Count);
        }

        [Fact]
        public void GraphContractStructure1()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.True(g.IsEmpty);

            INode s1 = g.CreateUriNode(new Uri("http://s1"));
            INode s2 = g.CreateUriNode(new Uri("http://s2"));
            INode p = g.CreateUriNode(new Uri("http://p"));
            INode o1 = g.CreateLiteralNode("value");
            INode o2 = g.CreateUriNode(new Uri("http://o"));

            INode[] vs = new INode[] { s1, s2, o1, o2 };
            INode[] es = new INode[] { p };

            g.Assert(new Triple(s1, p, o1));
            g.Assert(new Triple(s1, p, o2));
            g.Assert(new Triple(s2, p, o2));
            Assert.Equal(3, g.Count);

            // Vertices
            List<INode> actual = g.Vertices.ToList();
            Assert.Equal(vs.Length, actual.Count);
            Assert.True(vs.All(v => actual.Remove(v)));

            // Edges
            actual = g.Edges.ToList();
            Assert.Equal(es.Length, actual.Count);
            Assert.True(es.All(e => actual.Remove(e)));
        }

        [Fact]
        public void GraphContractStructure2()
        {
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.Equal(0, g.Count);
            Assert.True(g.IsEmpty);

            INode s1 = g.CreateBlankNode();
            INode s2 = g.CreateGraphLiteralNode();
            INode p = g.CreateUriNode(new Uri("http://p"));
            INode o1 = g.CreateVariableNode("var");
            INode o2 = g.CreateUriNode(new Uri("http://o"));

            INode[] vs = new INode[] { s1, s2, o1, o2 };
            INode[] es = new INode[] { p };

            g.Assert(new Triple(s1, p, o1));
            g.Assert(new Triple(s1, p, o2));
            g.Assert(new Triple(s2, p, o2));
            Assert.Equal(3, g.Count);

            // Vertices
            List<INode> actual = g.Vertices.ToList();
            Assert.Equal(vs.Length, actual.Count);
            Assert.True(vs.All(v => actual.Remove(v)));

            // Edges
            actual = g.Edges.ToList();
            Assert.Equal(es.Length, actual.Count);
            Assert.True(es.All(e => actual.Remove(e)));
        }

        [Fact]
        public void GraphContractUsage1()
        {
            //Create a new Empty Graph
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.NotNull(g);

            //Define Namespaces
            g.Namespaces.AddNamespace("vds", new Uri("http://www.vdesign-studios.com/dotNetRDF#"));
            g.Namespaces.AddNamespace("ecs", new Uri("http://id.ecs.soton.ac.uk/person/"));

            //Check we set the Namespace OK
            Assert.True(g.Namespaces.HasNamespace("vds"), "Failed to set a Namespace");

            //Create Uri Nodes
            INode rav08r, wh, lac, hcd;
            rav08r = g.CreateUriNode("ecs:11471");
            wh = g.CreateUriNode("ecs:1650");
            hcd = g.CreateUriNode("ecs:46");
            lac = g.CreateUriNode("ecs:60");

            //Create Uri Nodes for some Predicates
            INode supervises, collaborates, advises, has;
            supervises = g.CreateUriNode("vds:supervises");
            collaborates = g.CreateUriNode("vds:collaborates");
            advises = g.CreateUriNode("vds:advises");
            has = g.CreateUriNode("vds:has");

            //Create some Literal Nodes
            INode singleLine = g.CreateLiteralNode("Some string");
            INode multiLine = g.CreateLiteralNode("This goes over\n\nseveral\n\nlines");
            INode french = g.CreateLiteralNode("Bonjour", "fr");
            INode number = g.CreateLiteralNode("12", new Uri(g.Namespaces.GetNamespaceUri("xsd") + "integer"));

            g.Assert(new Triple(wh, supervises, rav08r));
            g.Assert(new Triple(lac, supervises, rav08r));
            g.Assert(new Triple(hcd, advises, rav08r));
            g.Assert(new Triple(wh, collaborates, lac));
            g.Assert(new Triple(wh, collaborates, hcd));
            g.Assert(new Triple(lac, collaborates, hcd));
            g.Assert(new Triple(rav08r, has, singleLine));
            g.Assert(new Triple(rav08r, has, multiLine));
            g.Assert(new Triple(rav08r, has, french));
            g.Assert(new Triple(rav08r, has, number));

            //Now print all the Statements
            Console.WriteLine("All Statements");
            Assert.Equal(10, g.Count);

            //Get statements about Rob Vesse
            Console.WriteLine();
            Console.WriteLine("Statements about Rob Vesse");
            Assert.Equal(7, g.GetTriples(rav08r).Count());

            //Get Statements about Collaboration
            Console.WriteLine();
            Console.WriteLine("Statements about Collaboration");
            Assert.Equal(3, g.GetTriples(collaborates).Count());
        }

        [Fact]
        public void GraphContractUsage2()
        {
            //Create a new Empty Graph
            IGraph g = this.CreateGraphInstance();
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            Assert.NotNull(g);

            //Define Namespaces
            g.Namespaces.AddNamespace("pets", new Uri("http://example.org/pets"));
            Assert.True(g.Namespaces.HasNamespace("pets"));

            //Create Uri Nodes
            INode dog, fido, rob, owner, name, species, breed, lab;
            dog = g.CreateUriNode("pets:Dog");
            fido = g.CreateUriNode("pets:abc123");
            rob = g.CreateUriNode("pets:def456");
            owner = g.CreateUriNode("pets:hasOwner");
            name = g.CreateUriNode("pets:hasName");
            species = g.CreateUriNode("pets:isAnimal");
            breed = g.CreateUriNode("pets:isBreed");
            lab = g.CreateUriNode("pets:Labrador");

            //Assert Triples
            g.Assert(new Triple(fido, species, dog));
            g.Assert(new Triple(fido, owner, rob));
            g.Assert(new Triple(fido, name, g.CreateLiteralNode("Fido")));
            g.Assert(new Triple(rob, name, g.CreateLiteralNode("Rob")));
            g.Assert(new Triple(fido, breed, lab));

            Assert.Equal(5, g.Count);
        }

        [Fact]
        public void GraphContractEvents1()
        {
            IEventedGraph g = this.CreateGraphInstance() as IEventedGraph;
            if (g == null || !g.HasEvents)
            {
                // Graph does not support events
                return;
            }
            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            // Attach event handler
            int events = 0;
            g.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                events++;
            };

            // Assert should generate an event
            Triple t = new Triple(g.CreateUriNode(new Uri("http://subject")), g.CreateUriNode(new Uri("http://predicate")), g.CreateBlankNode());
            g.Assert(t);
            Assert.Equal(1, events);

            // Adding the same triple again should not fire the event
            g.Assert(t);
            Assert.Equal(1, events);
        }

        [Fact]
        public void GraphContractEvents2()
        {
            IEventedGraph g = this.CreateGraphInstance() as IEventedGraph;
            if (g == null || !g.HasEvents)
            {
                // Graph does not support events
                return;
            }

            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            // Attach event handler
            int events = 0;
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Add;
            g.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(action, args.Action);
                events++;
            };

            // Assert should generate an event
            Triple t = new Triple(g.CreateUriNode(new Uri("http://subject")), g.CreateUriNode(new Uri("http://predicate")), g.CreateBlankNode());
            g.Assert(t);
            Assert.Equal(1, events);

            // Adding the same triple again should not fire the event
            g.Assert(t);
            Assert.Equal(1, events);

            // Retracting the triple should fire an event
            action = NotifyCollectionChangedAction.Remove;
            g.Retract(t);
            Assert.Equal(2, events);
        }

        [Fact]
        public void GraphContractEvents3()
        {
            IEventedGraph g = this.CreateGraphInstance() as IEventedGraph;
            if (g == null || !g.HasEvents)
            {
                // Graph does not support events
                return;
            }

            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            // Attach event handler
            int events = 0;
            g.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Reset, args.Action);
                events++;
            };

            // Clear 
            g.Clear();

            // Expect one event to have fired
            Assert.Equal(1, events);
        }

        [Fact]
        public void GraphContractEvents4()
        {
            IEventedGraph g = this.CreateGraphInstance() as IEventedGraph;
            if (g == null || !g.HasEvents)
            {
                // Graph does not support events
                return;
            }

            if (!this.ProvidesAccess(g, GraphAccessMode.ReadWrite)) return;

            // Attach event handler
            int events = 0, totalChanges = 0;
            NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Add;
            NotifyCollectionChangedEventArgs mostRecentArgs = null;
            g.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(action, args.Action);
                mostRecentArgs = args;
                events++;
                totalChanges += (args.NewItems != null ? args.NewItems.Count : 0) + (args.OldItems != null ? args.OldItems.Count : 0);
            };

            // Assert should generate an event
            const int numTriples = 1000;
            List<Triple> ts = this.GenerateTriples(numTriples).ToList();
            g.Assert(ts);
            switch (events)
            {
                case 1:
                    // Single bulk event
                    Assert.Equal(1, events);
                    Assert.Equal(numTriples, totalChanges);
                    Assert.NotNull(mostRecentArgs);
                    Assert.Equal(ts.Count, mostRecentArgs.NewItems.Count);
                    break;

                case numTriples:
                    // Event per triple
                    Assert.Equal(numTriples, events);
                    Assert.Equal(numTriples, totalChanges);
                    break;

                default:
                    // Some batching of events
                    Assert.Equal(numTriples, totalChanges);
                    break;
            }

            // Adding the same data again should not fire any events
            int currentEvents = events;
            g.Assert(ts);
            Assert.Equal(currentEvents, events);

            // Retracting the triple should fire an event
            events = 0;
            totalChanges = 0;
            action = NotifyCollectionChangedAction.Remove;
            mostRecentArgs = null;
            g.Retract(ts);
            switch (events)
            {
                case 1:
                    // Single bulk event
                    Assert.Equal(1, events);
                    Assert.Equal(numTriples, totalChanges);
                    Assert.NotNull(mostRecentArgs);
                    Assert.Equal(ts.Count, mostRecentArgs.OldItems.Count);
                    break;

                case numTriples:
                    // Event per triple
                    Assert.Equal(numTriples, events);
                    Assert.Equal(numTriples, totalChanges);
                    break;

                default:
                    // Some batching of events
                    Assert.Equal(numTriples, totalChanges);
                    break;
            }
        }
    }
}