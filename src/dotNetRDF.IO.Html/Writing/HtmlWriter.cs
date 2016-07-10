/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2012 dotNetRDF Project (dotnetrdf-developer@lists.sf.net)

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
using System.Linq;
using System.Text;
using System.IO;
using VDS.RDF.Collections;
using VDS.RDF.Graphs;
using VDS.RDF.Namespaces;
using VDS.RDF.Nodes;
using VDS.RDF.Query;
using VDS.RDF.Specifications;
using VDS.RDF.Writing.Contexts;

namespace VDS.RDF.Writing
{
    /// <summary>
    /// Class for saving RDF Graphs to a XHTML Table format with the actual Triples embedded as RDFa
    /// </summary>
    /// <remarks>
    /// <para>
    /// Since not all Triples can be embedded into XHTML those Triples will not have RDFa generated for them but all Triples will be shown in a human readable format.  Triples that can be serialized are roughly equivalent to anything that can be serialized in Turtle i.e. URI/BNode subject, URI predicates and URI/BNode/Literal object.
    /// </para>
    /// <para>
    /// If you encode Triples which have values datatyped as XML Literals with this writer then round-trip Graph equality is not guaranteed as the RDFa parser will add appropriate Namespace declarations to elements as required by the specification
    /// </para>
    /// </remarks>
    public class HtmlWriter
        : BaseHtmlWriter, IRdfWriter, INamespaceWriter
    {
        private INamespaceMapper _defaultNamespaces = new NamespaceMapper();

        /// <summary>
        /// Gets/Sets the Default Namespaces to use for writers
        /// </summary>
        public INamespaceMapper DefaultNamespaces
        {
            get
            {
                return this._defaultNamespaces;
            }
            set
            {
                this._defaultNamespaces = value;
            }
        }

        /// <summary>
        /// Saves the Result Set to the given Stream as an XHTML Table with embedded RDFa
        /// </summary>
        /// <param name="g">Graph to save</param>
        /// <param name="output">Stream to save to</param>
        public void Save(IGraph g, TextWriter output)
        {
            try
            {
                g.Namespaces.Import(this._defaultNamespaces);
                HtmlGraphWriterContext context = new HtmlGraphWriterContext(g, output);
                this.GenerateOutput(context);
                output.Close();
            }
            catch
            {
                try
                {
                    output.Close();
                }
                catch
                {
                    //No Catch Actions
                }
                throw;
            }
        }

        public void Save(IGraphStore graphStore, TextWriter output)
        {
            if (graphStore == null) throw new ArgumentNullException("graphStore", "Cannot write RDF from a null graph store");
            if (output == null) throw new ArgumentNullException("output", "Cannot write RDF to a null writer");

            // Grab the default graph (if any) and write it out
            IGraph g = graphStore.HasGraph(Quad.DefaultGraphNode) ? graphStore[Quad.DefaultGraphNode] : new Graph();
            this.Save(g, output);
        }

        /// <summary>
        /// Internal method which generates the HTML Output for the Graph
        /// </summary>
        /// <param name="context">Writer Context</param>
        private void GenerateOutput(HtmlGraphWriterContext context)
        {
            //Page Header
            context.HtmlWriter.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML+RDFa 1.0//EN\" \"http://www.w3.org/MarkUp/DTD/xhtml-rdfa-1.dtd\">");
            context.HtmlWriter.WriteLine();
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Html);
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Head);
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Title);
            context.HtmlWriter.WriteEncodedText("RDF Graph");
            // TODO Support a mechanism to pass Base URI to writers
            //if (context.BaseUri != null)
            //{
            //    context.HtmlWriter.WriteEncodedText(" - " + context.Graph.BaseUri.AbsoluteUri);
            //}
            context.HtmlWriter.RenderEndTag();
            if (!this.Stylesheet.Equals(String.Empty))
            {
                context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, this.Stylesheet);
                context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
                context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
                context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Link);
                context.HtmlWriter.RenderEndTag();
            }
            //TODO: Add <meta> for charset?
            context.HtmlWriter.RenderEndTag();
#if !NO_WEB
            context.HtmlWriter.WriteLine();
#endif

            //Start Body
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Body);

            //Title
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.H3);
            context.HtmlWriter.WriteEncodedText("RDF Graph");
            // TODO Support a mechanism to pass Base URI to writers
            //if (context.Graph.BaseUri != null)
            //{
            //    context.HtmlWriter.WriteEncodedText(" - " + context.Graph.BaseUri.AbsoluteUri);
            //}
            context.HtmlWriter.RenderEndTag();
#if !NO_WEB
            context.HtmlWriter.WriteLine();
#endif

            //Create a Table for the Graph
            context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Table);

            //Create a Table Header
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Thead);
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Th);
            context.HtmlWriter.WriteEncodedText("Subject");
            context.HtmlWriter.RenderEndTag();
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Th);
            context.HtmlWriter.WriteEncodedText("Predicate");
            context.HtmlWriter.RenderEndTag();
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Th);
            context.HtmlWriter.WriteEncodedText("Object");
            context.HtmlWriter.RenderEndTag();
            context.HtmlWriter.RenderEndTag();
            context.HtmlWriter.RenderEndTag();
#if !NO_WEB
            context.HtmlWriter.WriteLine();
#endif

            //Create a Table Body for the Triple
            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Tbody);

            TripleCollection triplesDone = new TripleCollection();
            foreach (INode subj in context.Graph.Triples.Select(t => t.Subject))
            {
                IEnumerable<Triple> ts = context.Graph.GetTriplesWithSubject(subj);

                //Start a Row
                context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
                context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);

                //Then a Column for the Subject which spans the correct number of Rows
                context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Rowspan, ts.Count().ToString());

                context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
#if !NO_WEB
                context.HtmlWriter.WriteLine();
#endif
                //For each Subject add an anchor if it can be reduced to a QName
                if (subj.NodeType == NodeType.Uri)
                {
                    String qname;
                    if (context.QNameMapper.ReduceToPrefixedName(subj.ToString(), out qname))
                    {
                        if (!qname.EndsWith(":"))
                        {
                            qname = qname.Substring(qname.IndexOf(':') + 1);
                            context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Name, qname);
                            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                            context.HtmlWriter.RenderEndTag();
                        }
                    }
                }

                this.GenerateNodeOutput(context, subj);
#if !NO_WEB
                context.HtmlWriter.WriteLine();
#endif
                context.HtmlWriter.RenderEndTag();
#if !NO_WEB
                context.HtmlWriter.WriteLine();
#endif

                bool firstPred = true;
                foreach (Triple t in ts)
                {
                    if (triplesDone.Contains(t)) continue;
                    if (!firstPred)
                    {
                        //If not the first Triple start a new row
                        context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
                        context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
                    }

                    //Then a Column for the Predicate
                    IEnumerable<Triple> predTriples = context.Graph.GetTriplesWithSubjectPredicate(t.Subject, t.Predicate);
                    context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Rowspan, predTriples.Count().ToString());
                    context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
#if !NO_WEB
                    context.HtmlWriter.WriteLine();
#endif
                    this.GenerateNodeOutput(context, t.Predicate);
#if !NO_WEB
                    context.HtmlWriter.WriteLine();
#endif
                    context.HtmlWriter.RenderEndTag();
#if !NO_WEB
                    context.HtmlWriter.WriteLine();
#endif

                    //Then we write out all the Objects
                    bool firstObj = true;
                    foreach (Triple predTriple in predTriples)
                    {
                        if (triplesDone.Contains(predTriple)) continue;
                        if (!firstObj)
                        {
                            //If not the first Triple start a new row
                            context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
                            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
                        }

                        //Object Column
                        context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
#if !NO_WEB
                        context.HtmlWriter.WriteLine();
#endif
                        this.GenerateNodeOutput(context, predTriple.Object, predTriple);
#if !NO_WEB
                        context.HtmlWriter.WriteLine();
#endif
                        context.HtmlWriter.RenderEndTag();
#if !NO_WEB
                        context.HtmlWriter.WriteLine();
#endif

                        //End of Row
                        context.HtmlWriter.RenderEndTag();
#if !NO_WEB
                        context.HtmlWriter.WriteLine();
#endif
                        firstObj = false;

                        triplesDone.Add(predTriple);
                    }
                    firstPred = false;
                }
            }


            //End Table Body
            context.HtmlWriter.RenderEndTag();

            //End Table
            context.HtmlWriter.RenderEndTag();

            //End of Page
            context.HtmlWriter.RenderEndTag(); //End Body
            context.HtmlWriter.RenderEndTag(); //End Html
        }

        /// <summary>
        /// Generates Output for a given Node
        /// </summary>
        /// <param name="context">Writer Context</param>
        /// <param name="n">Node</param>
        private void GenerateNodeOutput(HtmlGraphWriterContext context, INode n)
        {
            this.GenerateNodeOutput(context, n, null);
        }

        /// <summary>
        /// Generates Output for a given Node
        /// </summary>
        /// <param name="context">Writer Context</param>
        /// <param name="n">Node</param>
        /// <param name="t">Triple being written</param>
        private void GenerateNodeOutput(HtmlGraphWriterContext context, INode n, Triple t)
        {
            //Embed RDFa on the Node Output
            bool rdfASerializable = false;
            if (t != null)
            {
                if (t.Predicate.NodeType == NodeType.Uri)
                {
                    //Use @about to specify the Subject
                    if (t.Subject.NodeType == NodeType.Uri)
                    {
                        rdfASerializable = true;
                        context.HtmlWriter.AddAttribute("about", context.UriFormatter.FormatUri(t.Subject.ToString()));
                    }
                    else if (t.Subject.NodeType == NodeType.Blank)
                    {
                        rdfASerializable = true;
                        context.HtmlWriter.AddAttribute("about", "[" + t.Subject.ToString() + "]");
                    }
                    else
                    {
                        this.RaiseWarning("Cannot serialize a Triple since the Subject is not a URI/Blank Node: " + t.Subject.ToString());
                    }

                    //Then if we can serialize this Triple we serialize the Predicate
                    if (rdfASerializable)
                    {
                        //Get the CURIE for the Predicate
                        String curie;
                        String tempNamespace;
                        if (context.QNameMapper.ReduceToPrefixedName(t.Predicate.ToString(), out curie, out tempNamespace))
                        {
                            //Extract the Namespace and make sure it's registered on this Attribute
                            String ns = curie.Substring(0, curie.IndexOf(':'));
                            context.HtmlWriter.AddAttribute("xmlns:" + ns, context.UriFormatter.FormatUri(context.QNameMapper.GetNamespaceUri(ns)));
                        }
                        else
                        {
                            this.RaiseWarning("Cannot serialize a Triple since the Predicate cannot be reduced to a QName: " + t.Predicate.ToString());
                            rdfASerializable = false;
                        }

                        if (rdfASerializable)
                        {
                            switch (t.Object.NodeType)
                            {
                                case NodeType.Blank:
                                case NodeType.Uri:
                                    //If the Object is a URI or a Blank then we specify the predicate with @rel
                                    context.HtmlWriter.AddAttribute("rel", curie);
                                    break;

                                case NodeType.Literal:
                                    //If the Object is a Literal we specify the predicate with @property
                                    context.HtmlWriter.AddAttribute("property", curie);
                                    break;
                                default:
                                    this.RaiseWarning("Cannot serialize a Triple since the Object is not a URI/Blank/Literal Node: " + t.Object.ToString());
                                    rdfASerializable = false;
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    this.RaiseWarning("Cannot serialize a Triple since the Predicate is not a URI Node: " + t.Predicate.ToString());
                }
            }

            String qname;
            switch (n.NodeType)
            {
                case NodeType.Blank:
                    if (rdfASerializable)
                    {
                        //Need to embed the CURIE for the BNode in the @resource attribute
                        context.HtmlWriter.AddAttribute("resource", "[" + n.ToString() + "]");
                    }

                    context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClassBlankNode);
                    context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
                    context.HtmlWriter.WriteEncodedText(n.ToString());
                    context.HtmlWriter.RenderEndTag();
                    break;

                case NodeType.Literal:
                    if (n.HasLanguage || !n.HasDataType)
                    {
                        if (rdfASerializable)
                        {
                            if (n.HasLanguage)
                            {
                                //Need to add the language as an xml:lang attribute
                                context.HtmlWriter.AddAttribute("xml:lang", n.Language);
                            }
                        }
                        context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClassLiteral);
                        context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
                        context.HtmlWriter.WriteEncodedText(n.Value);
                        context.HtmlWriter.RenderEndTag();
                        if (n.HasLanguage)
                        {
                            context.HtmlWriter.WriteEncodedText("@");
                            context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClassLangSpec);
                            context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
                            context.HtmlWriter.WriteEncodedText(n.Language);
                            context.HtmlWriter.RenderEndTag();
                        }
                    }
                    else
                    {
                        if (rdfASerializable)
                        {
                            //Need to embed the datatype in the @datatype attribute
                            String dtcurie, dtnamespace;
                            if (context.QNameMapper.ReduceToPrefixedName(n.DataType.AbsoluteUri, out dtcurie, out dtnamespace))
                            {
                                //Extract the Namespace and make sure it's registered on this Attribute
                                String ns = dtcurie.Substring(0, dtcurie.IndexOf(':'));
                                context.HtmlWriter.AddAttribute("xmlns:" + ns, context.UriFormatter.FormatUri(context.QNameMapper.GetNamespaceUri(ns)));
                                context.HtmlWriter.AddAttribute("datatype", dtcurie);
                            }
                        }

                        context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClassLiteral);
                        context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
                        if (n.DataType.AbsoluteUri.Equals(RdfSpecsHelper.RdfXmlLiteral))
                        {
                            context.HtmlWriter.Write(n.Value);
                        }
                        else
                        {
                            context.HtmlWriter.WriteEncodedText(n.Value);
                        }
                        context.HtmlWriter.RenderEndTag();

                        //Output the Datatype
                        context.HtmlWriter.WriteEncodedText("^^");
                        context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, n.DataType.AbsoluteUri);
                        context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClassDatatype);
                        context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                        if (context.QNameMapper.ReduceToPrefixedName(n.DataType.AbsoluteUri, out qname))
                        {
                            context.HtmlWriter.WriteEncodedText(qname);
                        }
                        else
                        {
                            context.HtmlWriter.WriteEncodedText(n.DataType.AbsoluteUri);
                        }
                        context.HtmlWriter.RenderEndTag();
                    }
                    break;

                case NodeType.GraphLiteral:
                    //Error
                    throw new RdfOutputException(WriterErrorMessages.GraphLiteralsUnserializable("HTML"));

                case NodeType.Uri:
                    if (rdfASerializable && !this.UriPrefix.Equals(String.Empty))
                    {
                        //If the URIs are being prefixed with something then we need to set the original
                        //URI in the resource attribute to generate the correct triple
                        context.HtmlWriter.AddAttribute("resource", n.ToString());
                    }

                    context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClassUri);
                    context.HtmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, this.UriPrefix + n.ToString());
                    context.HtmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                    if (context.QNameMapper.ReduceToPrefixedName(n.ToString(), out qname))
                    {
                        context.HtmlWriter.WriteEncodedText(qname);
                    }
                    else
                    {
                        context.HtmlWriter.WriteEncodedText(n.ToString());
                    }
                    context.HtmlWriter.RenderEndTag();
                    break;

                default:
                    throw new RdfOutputException(WriterErrorMessages.UnknownNodeTypeUnserializable("HTML"));
            }
        }

        /// <summary>
        /// Helper method for raising the <see cref="Warning">Warning</see> event
        /// </summary>
        /// <param name="message">Warning Message</param>
        private void RaiseWarning(String message)
        {
            RdfWriterWarning d = this.Warning;
            if (d != null)
            {
                d(message);
            }
        }

        /// <summary>
        /// Event which is raised if there is a non-fatal error with the RDF being output
        /// </summary>
        public event RdfWriterWarning Warning;

        /// <summary>
        /// Gets the String representation of the writer which is a description of the syntax it produces
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "XHTML+RDFa";
        }
    }
}