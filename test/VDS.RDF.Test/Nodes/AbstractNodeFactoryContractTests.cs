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
using System.Linq;
using Xunit;
using VDS.RDF.Graphs;
using VDS.RDF.Namespaces;
using VDS.RDF.Parsing;
using VDS.RDF.Specifications;

namespace VDS.RDF.Nodes
{
    public abstract class AbstractNodeFactoryContractTests
        : BaseTest
    {
        protected readonly Uri _xsdString = new Uri(XmlSpecsHelper.XmlSchemaDataTypeString);
        protected readonly Uri _rdfLangString = new Uri(RdfSpecsHelper.RdfLangString);

        protected readonly int _iterations = 1000;
        protected readonly UriComparer _uriComparer = new UriComparer();

        /// <summary>
        /// Creates a fresh new factory instance for use in testing
        /// </summary>
        /// <returns>New factory instance</returns>
        protected abstract INodeFactory CreateFactoryInstance();

        private readonly INamespaceMapper _nsMapper = new NamespaceMapper();

        protected AbstractNodeFactoryContractTests()
        {
            this._nsMapper.AddNamespace("", new Uri("http://example.org/"));
        }

        /// <summary>
        /// Provides QName to URI resolution for some common namespaces used in the tests
        /// </summary>
        /// <param name="qname">QName</param>
        /// <returns>URI</returns>
        protected Uri ResolveQName(String qname)
        {
            return UriFactory.ResolvePrefixedName(qname, this._nsMapper, null);
        }

        [Fact]
        public void NodeFactoryContractBlanks01()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            // Different blank node creation calls should result in different blank nodes
            INode a = factory.CreateBlankNode();
            Assert.Equal(NodeType.Blank, a.NodeType);

            INode b = factory.CreateBlankNode();
            Assert.Equal(NodeType.Blank, b.NodeType);

            Assert.NotEqual(a, b, "Different calls to CreateBlankNode() MUST produce different blank nodes");
        }

        [Fact]
        public void NodeFactoryContractBlanks02()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            // Different blank node creation calls should result in different blank nodes
            INode a = factory.CreateBlankNode();
            Assert.Equal(NodeType.Blank, a.NodeType);

            for (int i = 0; i < this._iterations; i++)
            {
                INode b = factory.CreateBlankNode();
                Assert.Equal(NodeType.Blank, b.NodeType);

                Assert.NotEqual(a, b, "Different calls to CreateBlankNode() MUST produce different blank nodes");
            }
        }

        [Fact]
        public void NodeFactoryContractBlanks03()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            // Passing the same ID in should result in the same blank node
            Guid id = Guid.NewGuid();
            INode a = factory.CreateBlankNode(id);
            Assert.Equal(NodeType.Blank, a.NodeType);
            Assert.Equal(id, a.AnonID);

            INode b = factory.CreateBlankNode(id);
            Assert.Equal(NodeType.Blank, b.NodeType);
            Assert.Equal(id, b.AnonID);

            Assert.Equal(a, b, "Calls to CreateBlankNode() with the same Guid MUST produce equivalent blank nodes");
        }

        [Fact]
        public void NodeFactoryContractBlanks04()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            // Passing different IDs in should result in the same blank node
            Guid id = Guid.NewGuid();
            INode a = factory.CreateBlankNode(id);
            Assert.Equal(NodeType.Blank, a.NodeType);
            Assert.Equal(id, a.AnonID);

            id = Guid.NewGuid();
            INode b = factory.CreateBlankNode(id);
            Assert.Equal(NodeType.Blank, b.NodeType);
            Assert.Equal(id, b.AnonID);

            Assert.NotEqual(a, b, "Calls to CreateBlankNode() with different Guids MUST produce different blank nodes");
        }

        [Fact]
        public void NodeFactoryContractBlanks03B()
        {
            INodeFactory f1 = this.CreateFactoryInstance();
            INodeFactory f2 = this.CreateFactoryInstance();

            // Passing the same ID in should result in the same blank node
            Guid id = Guid.NewGuid();
            INode a = f1.CreateBlankNode(id);
            Assert.Equal(NodeType.Blank, a.NodeType);
            Assert.Equal(id, a.AnonID);

            INode b = f2.CreateBlankNode(id);
            Assert.Equal(NodeType.Blank, b.NodeType);
            Assert.Equal(id, b.AnonID);

            Assert.Equal(a, b, "Calls to CreateBlankNode() with the same Guid MUST produce equivalent blank nodes across factories");
        }

        [Fact]
        public void NodeFactoryContractBlanks05()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            // Passing the same ID in should result in the same blank node
            Guid id = Guid.NewGuid();
            INode a = factory.CreateBlankNode(id);
            Assert.Equal(NodeType.Blank, a.NodeType);
            Assert.Equal(id, a.AnonID);

            for (int i = 0; i < this._iterations; i++)
            {
                INode b = factory.CreateBlankNode(id);
                Assert.Equal(NodeType.Blank, b.NodeType);
                Assert.Equal(id, b.AnonID);

                Assert.Equal(a, b, "Calls to CreateBlankNode() with the same Guid MUST produce equivalent blank nodes");
            }
        }

        [Fact]
        public void NodeFactoryContractBlanks06()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            // Passing different IDs in should result in the same blank node
            Guid id = Guid.NewGuid();
            INode a = factory.CreateBlankNode(id);
            Assert.Equal(NodeType.Blank, a.NodeType);
            Assert.Equal(id, a.AnonID);

            for (int i = 0; i < this._iterations; i++)
            {
                id = Guid.NewGuid();
                INode b = factory.CreateBlankNode(id);
                Assert.Equal(NodeType.Blank, b.NodeType);
                Assert.Equal(id, b.AnonID);

                Assert.NotEqual(a, b, "Calls to CreateBlankNode() with different Guids MUST produce different blank nodes");
            }
        }

        [Fact]
        public void NodeFactoryContractUris01()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            // Creating URI nodes with same URI should be equal
            Uri u = this.ResolveQName(":test");
            INode a = factory.CreateUriNode(u);
            Assert.Equal(NodeType.Uri, a.NodeType);
            Assert.True(this._uriComparer.Equals(u, a.Uri), "Creating a URI Node MUST preserve the given URI as-is");

            INode b = factory.CreateUriNode(u);
            Assert.Equal(NodeType.Uri, b.NodeType);
            Assert.True(this._uriComparer.Equals(u, b.Uri), "Creating a URI Node MUST preserve the given URI as-is");

            Assert.Equal(a, b, "Calls to CreateUriNode() with the same URI MUST produce equivalent URI nodes");
        }

        [Fact]
        public void NodeFactoryContractUris02()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            // Creating URI nodes with same URI should be equal
            Uri u = this.ResolveQName(":test");
            INode a = factory.CreateUriNode(u);
            Assert.Equal(NodeType.Uri, a.NodeType);
            Assert.True(this._uriComparer.Equals(u, a.Uri), "Creating a URI Node MUST preserve the given URI as-is");

            for (int i = 0; i < this._iterations; i++)
            {
                INode b = factory.CreateUriNode(u);
                Assert.Equal(NodeType.Uri, b.NodeType);
                Assert.True(this._uriComparer.Equals(u, b.Uri), "Creating a URI Node MUST preserve the given URI as-is");

                Assert.Equal(a, b, "Calls to CreateUriNode() with the same URI MUST produce equivalent URI nodes");
            }
        }

        [Fact]
        public void NodeFactoryContractUris03()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            // Creating URI nodes with different URI should not be equal
            Uri u = this.ResolveQName(":test");
            INode a = factory.CreateUriNode(u);
            Assert.Equal(NodeType.Uri, a.NodeType);
            Assert.True(this._uriComparer.Equals(u, a.Uri), "Creating a URI Node MUST preserve the given URI as-is");

            u = this.ResolveQName(":other");
            INode b = factory.CreateUriNode(u);
            Assert.Equal(NodeType.Uri, b.NodeType);
            Assert.True(this._uriComparer.Equals(u, b.Uri), "Creating a URI Node MUST preserve the given URI as-is");

            Assert.NotEqual(a, b, "Calls to CreateUriNode() with different URIs MUST produce different URI nodes");
        }

        [Fact]
        public void NodeFactoryContractUris04()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            // Creating URI nodes with different URI should not be equal
            Uri u = this.ResolveQName(":test");
            INode a = factory.CreateUriNode(u);
            Assert.Equal(NodeType.Uri, a.NodeType);
            Assert.True(this._uriComparer.Equals(u, a.Uri), "Creating a URI Node MUST preserve the given URI as-is");

            for (int i = 0; i < this._iterations; i++)
            {
                u = this.ResolveQName(":other" + i);
                INode b = factory.CreateUriNode(u);
                Assert.Equal(NodeType.Uri, b.NodeType);
                Assert.True(this._uriComparer.Equals(u, b.Uri), "Creating a URI Node MUST preserve the given URI as-is");

                Assert.NotEqual(a, b, "Calls to CreateUriNode() with different URIs MUST produce different URI nodes");
            }
        }

        [Fact]
        public void NodeFactoryContractLiterals01()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            INode a = factory.CreateLiteralNode("test");
            Assert.Equal(NodeType.Literal, a.NodeType);
            Assert.Equal("test", a.Value);
            Assert.False(a.HasLanguage);
            Assert.Null(a.Language);

            if (factory.CreatesImplicitlyTypedLiterals)
            {
                // Expect implicit typing as xsd:string
                Assert.True(a.HasDataType);
                Assert.True(this._uriComparer.Equals(this._xsdString, a.DataType), "A factory that creates RDF 1.1 literals must implicitly type literals as xsd:string if no type is provided");
            }
            else
            {
                Assert.False(a.HasDataType);
                Assert.Null(a.DataType);
            }

            INode b = factory.CreateLiteralNode("test");
            Assert.Equal("test", b.Value);
            Assert.False(b.HasLanguage);
            Assert.Null(b.Language);

            if (factory.CreatesImplicitlyTypedLiterals)
            {
                // Expect implicit typing as xsd:string
                Assert.True(b.HasDataType);
                Assert.True(this._uriComparer.Equals(this._xsdString, b.DataType), "A factory that creates RDF 1.1 literals must implicitly type literals as xsd:string if no type is provided");
            }
            else
            {
                Assert.False(b.HasDataType);
                Assert.Null(b.DataType);
            }

            Assert.Equal(a, b, "Calling CreateLiteralNode() with the same parameters MUST result in equivalent literal nodes");
        }

        [Fact]
        public void NodeFactoryContractLiterals02()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            INode a = factory.CreateLiteralNode("test", "en");
            Assert.Equal(NodeType.Literal, a.NodeType);
            Assert.Equal("test", a.Value);
            Assert.True(a.HasLanguage);
            Assert.Equal("en", a.Language);

            if (factory.CreatesImplicitlyTypedLiterals)
            {
                // Expect implicit typing as rdf:langString
                Assert.True(a.HasDataType);
                Assert.True(this._uriComparer.Equals(this._rdfLangString, a.DataType), "A factory that creates RDF 1.1 literals must implicitly type literals as xsd:string if no type is provided");
            }
            else
            {
                Assert.False(a.HasDataType);
                Assert.Null(a.DataType);
            }

            INode b = factory.CreateLiteralNode("test", "en");
            Assert.Equal(NodeType.Literal, b.NodeType);
            Assert.Equal("test", b.Value);
            Assert.True(b.HasLanguage);
            Assert.Equal("en", b.Language);

            if (factory.CreatesImplicitlyTypedLiterals)
            {
                // Expect implicit typing as rdf:langString
                Assert.True(b.HasDataType);
                Assert.True(this._uriComparer.Equals(this._rdfLangString, b.DataType), "A factory that creates RDF 1.1 literals must implicitly type literals as xsd:string if no type is provided");
            }
            else
            {
                Assert.False(b.HasDataType);
                Assert.Null(b.DataType);
            }

            Assert.Equal(a, b, "Calling CreateLiteralNode() with the same parameters MUST result in equivalent literal nodes");
        }

        [Fact]
        public void NodeFactoryContractLiterals03()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            Uri dt = this.ResolveQName(":type");
            INode a = factory.CreateLiteralNode("test", dt);
            Assert.Equal(NodeType.Literal, a.NodeType);
            Assert.Equal("test", a.Value);
            Assert.False(a.HasLanguage);
            Assert.Null(a.Language);
            Assert.True(a.HasDataType);
            Assert.True(this._uriComparer.Equals(dt, a.DataType));

            INode b = factory.CreateLiteralNode("test", dt);
            Assert.Equal(NodeType.Literal, b.NodeType);
            Assert.Equal("test", b.Value);
            Assert.False(b.HasLanguage);
            Assert.Null(b.Language);
            Assert.True(b.HasDataType);
            Assert.True(this._uriComparer.Equals(dt, b.DataType));
            
            Assert.Equal(a, b, "Calling CreateLiteralNode() with the same parameters MUST result in equivalent literal nodes");
        }

        [Fact]
        public void NodeFactoryContractLiterals04()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            INode implicitLit = factory.CreateLiteralNode("test");
            INode explicitLit = factory.CreateLiteralNode("test", this._xsdString);

            if (factory.CreatesImplicitlyTypedLiterals)
            {
                Assert.Equal(implicitLit, explicitLit, "Calling CreateLiteralNode() with equivalent parameters when implicit typing is supported MUST result in equivalent literal nodes");
            }
            else
            {
                Assert.NotEqual(implicitLit, explicitLit, "Calling CreateLiteralNode() with equivalent parameters when implicit typing is not supported MUST result in different literal nodes");
            }
        }

        [Fact]
        public void NodeFactoryContractLiterals04B()
        {
            INodeFactory factory = this.CreateFactoryInstance();

            INode implicitLit = factory.CreateLiteralNode("test");
            INode explicitLit = factory.CreateLiteralNode("test", this.ResolveQName("xsd:string"));

            if (factory.CreatesImplicitlyTypedLiterals)
            {
                Assert.Equal(implicitLit, explicitLit, "Calling CreateLiteralNode() with equivalent parameters when implicit typing is supported MUST result in equivalent literal nodes");
            }
            else
            {
                Assert.NotEqual(implicitLit, explicitLit, "Calling CreateLiteralNode() with equivalent parameters when implicit typing is not supported MUST result in different literal nodes");
            }
        }

        [Fact]
        public void NodeFactoryContractDistinct()
        {
            INodeFactory factory = this.CreateFactoryInstance();
            List<INode> test = new List<INode>()
                               {
                                   factory.CreateUriNode(this.ResolveQName("rdf:type")),
                                   factory.CreateUriNode(new Uri("http://example.org")),
                                   factory.CreateBlankNode(),
                                   // Will be distinct from previous CreateBlankNode()
                                   factory.CreateBlankNode(),
                                   null,
                                   factory.CreateBlankNode(Guid.NewGuid()),
                                   factory.CreateLiteralNode("Test text"),
                                   // Will be distinct from CreateLiteralNode("Test text") because it has a languate type
                                   factory.CreateLiteralNode("Test text", "en"), 
                                   // Non-distinct - Equivalent to CreateLiteralNode("Test text") due to RDF 1.1 implicit typing to xsd:string
                                   factory.CreateLiteralNode("Test text", new Uri(XmlSpecsHelper.XmlSchemaDataTypeString)), 
                                   // Non-distinct, already seen earlier in the list
                                   factory.CreateUriNode(this.ResolveQName("rdf:type")), 
                                   // Non-distinct, already seen earlier in the list
                                   null,
                                   factory.CreateUriNode(new Uri("http://example.org#test")),
                                   // Non-distinct, already seen earlier in the list
                                   factory.CreateUriNode(new Uri("http://example.org"))
                               };
            const int totalNonDistinct = 4;

            foreach (INode n in test.Distinct())
            {
                Console.WriteLine(n != null ? n.ToString() : "null");
            }
            Assert.Equal(test.Count - totalNonDistinct, test.Distinct().Count());
        }

        [Fact]
        public void NodeFactoryContractHashCodes()
        {
            Console.WriteLine("Tests that Literal and URI Nodes produce different Hashes");
            Console.WriteLine();

            // Create the Nodes
            INodeFactory factory = this.CreateFactoryInstance();
            INode u = factory.CreateUriNode(new Uri("http://www.google.com"));
            INode l = factory.CreateLiteralNode("http://www.google.com/");

            Console.WriteLine("Created a URI and Literal Node both referring to 'http://www.google.com'");
            Console.WriteLine("String form of URI Node is:");
            Console.WriteLine(u.ToString());
            Console.WriteLine("String form of Literal Node is:");
            Console.WriteLine(l.ToString());
            Console.WriteLine("Hash Code of URI Node is " + u.GetHashCode());
            Console.WriteLine("Hash Code of Literal Node is " + l.GetHashCode());
            Console.WriteLine("Hash Codes are Equal? " + u.GetHashCode().Equals(l.GetHashCode()));
            Console.WriteLine("Nodes are equal? " + u.Equals(l));

            Assert.NotEqual(u.GetHashCode(), l.GetHashCode());
            Assert.NotEqual(u, l);

            //Create some plain and typed literals which may have colliding Hash Codes
            INode plain = factory.CreateLiteralNode("test^^http://example.org/type");
            INode typed = factory.CreateLiteralNode("test", new Uri("http://example.org/type"));

            Console.WriteLine();
            Console.WriteLine("Created a Plain and Typed Literal where the ToString() representations may be similar");
            Console.WriteLine("Plain Literal String form is:");
            Console.WriteLine(plain.ToString());
            Console.WriteLine("Typed Literal String from is:");
            Console.WriteLine(typed.ToString());
            Console.WriteLine("Hash Code of Plain Literal is " + plain.GetHashCode());
            Console.WriteLine("Hash Code of Typed Literal is " + typed.GetHashCode());
            Console.WriteLine("Hash Codes are Equal? " + plain.GetHashCode().Equals(typed.GetHashCode()));
            Console.WriteLine("Nodes are equal? " + plain.Equals(typed));

            Assert.NotEqual(plain.GetHashCode(), typed.GetHashCode());
            Assert.NotEqual(plain, typed);

            //Create Triples
            INode b = factory.CreateBlankNode();
            INode type = factory.CreateUriNode(this.ResolveQName("rdf:type"));
            Triple t1 = new Triple(b, type, u);
            Triple t2 = new Triple(b, type, l);

            Console.WriteLine();
            Console.WriteLine("Created two Triples stating a Blank Node has rdf:type of the URI Nodes created earlier");
            Console.WriteLine("String form of Triple 1 (using URI Node) is:");
            Console.WriteLine(t1.ToString());
            Console.WriteLine("String form of Triple 2 (using Literal Node) is:");
            Console.WriteLine(t2.ToString());
            Console.WriteLine("Hash Code of Triple 1 is " + t1.GetHashCode());
            Console.WriteLine("Hash Code of Triple 2 is " + t2.GetHashCode());
            Console.WriteLine("Hash Codes are Equal? " + t1.GetHashCode().Equals(t2.GetHashCode()));
            Console.WriteLine("Triples are Equal? " + t1.Equals(t2));

            Assert.NotEqual(t1.GetHashCode(), t2.GetHashCode());
            Assert.NotEqual(t1, t2);

            //Create Triples from the earlier Literal Nodes
            t1 = new Triple(b, type, plain);
            t2 = new Triple(b, type, typed);

            Console.WriteLine();
            Console.WriteLine("Created two Triples stating a Blank Node has rdf:type of the Literal Nodes created earlier");
            Console.WriteLine("String form of Triple 1 (using Plain Literal) is:");
            Console.WriteLine(t1.ToString());
            Console.WriteLine("String form of Triple 2 (using Typed Literal) is:");
            Console.WriteLine(t2.ToString());
            Console.WriteLine("Hash Code of Triple 1 is " + t1.GetHashCode());
            Console.WriteLine("Hash Code of Triple 2 is " + t2.GetHashCode());
            Console.WriteLine("Hash Codes are Equal? " + t1.GetHashCode().Equals(t2.GetHashCode()));
            Console.WriteLine("Triples are Equal? " + t1.Equals(t2));

            Assert.NotEqual(t1.GetHashCode(), t2.GetHashCode());
            Assert.NotEqual(t1, t2);
        }

        [Fact]
        public void NodeFactoryContractUriNodeEquality()
        {
            //Create the Nodes
            INodeFactory factory = this.CreateFactoryInstance();
            Console.WriteLine("Creating two URIs referring to google - one lowercase, one uppercase - which should be equivalent");
            INode a = factory.CreateUriNode(new Uri("http://www.google.com"));
            INode b = factory.CreateUriNode(new Uri("http://www.GOOGLE.com/"));

            TestTools.CompareNodes(a, b, true);

            Console.WriteLine("Creating two URIs with the same Fragment ID but differing in case and thus are different since Fragment IDs are case sensitive => not equals");
            INode c = factory.CreateUriNode(new Uri("http://www.google.com/#Test"));
            INode d = factory.CreateUriNode(new Uri("http://www.GOOGLE.com/#test"));

            TestTools.CompareNodes(c, d, false);

            Console.WriteLine("Creating two identical URIs with unusual characters in them");
            INode e = factory.CreateUriNode(new Uri("http://www.google.com/random,_@characters"));
            INode f = factory.CreateUriNode(new Uri("http://www.google.com/random,_@characters"));

            TestTools.CompareNodes(e, f, true);

            Console.WriteLine("Creating two URIs with similar paths that differ in case");
            INode h = factory.CreateUriNode(new Uri("http://www.google.com/path/test/case"));
            INode i = factory.CreateUriNode(new Uri("http://www.google.com/path/Test/case"));

            TestTools.CompareNodes(h, i, false);

            Console.WriteLine("Creating three URIs with equivalent relative paths");
            INode j = factory.CreateUriNode(new Uri("http://www.google.com/relative/test/../example.html"));
            INode k = factory.CreateUriNode(new Uri("http://www.google.com/relative/test/monkey/../../example.html"));
            INode l = factory.CreateUriNode(new Uri("http://www.google.com/relative/./example.html"));

            TestTools.CompareNodes(j, k, true);
            TestTools.CompareNodes(k, l, true);
        }

        [Fact]
        public void NodeFactoryContractBlankNodeEquality()
        {
            INodeFactory factory1 = this.CreateFactoryInstance();
            INodeFactory factory2 = this.CreateFactoryInstance();
            INodeFactory factory3 = this.CreateFactoryInstance();

            Console.WriteLine("Doing some Blank Node Equality Testing");

            INode b = factory1.CreateBlankNode();
            INode c = factory1.CreateBlankNode();
            INode d = factory2.CreateBlankNode();
            INode e = factory3.CreateBlankNode();

            //Shouldn't be equal
            Assert.NotEqual(b, c, "Two Anonymous Blank Nodes created by same factory should be non-equal");
            Assert.NotEqual(c, d, "Anonymous Blank Nodes created by different factories should be non-equal");
            Assert.NotEqual(b, d, "Anonymous Blank Nodes created by different factories should be non-equal");
            Assert.NotEqual(d, e, "Anonymous Blank Nodes created by different factories should be non-equal");

            //Should be equal
            Assert.Equal(b, b, "A Blank Node should be equal to itself");
            Assert.Equal(c, c, "A Blank Node should be equal to itself");
            Assert.Equal(d, d, "A Blank Node should be equal to itself");
            Assert.Equal(e, e, "A Blank Node should be equal to itself");

            //Named Nodes
            Guid guid = Guid.NewGuid();
            INode one = factory1.CreateBlankNode(guid);
            INode two = factory2.CreateBlankNode(guid);
            INode three = factory3.CreateBlankNode(guid);

            Assert.Equal(one, three, "Blank nodes with same ID are equal regardless of factory");
            Assert.Equal(one, two, "Blank nodes with same ID are equal regardless of factory");
            Assert.Equal(two, three, "Blank nodes with same ID are equal regardless of factory");
        }

        [Fact]
        public void NodeFactoryContractLiteralNodeEquality()
        {
            try
            {
                INodeFactory factory = this.CreateFactoryInstance();

                //Strict Mode Tests
                Console.WriteLine("Doing a load of Strict Literal Equality Tests");
                Options.LiteralEqualityMode = LiteralEqualityMode.Strict;

                //Test Literals with Language Tags
                INode hello, helloEn, helloEnUS, helloAgain;
                hello = factory.CreateLiteralNode("hello");
                helloEn = factory.CreateLiteralNode("hello", "en");
                helloEnUS = factory.CreateLiteralNode("hello", "en-US");
                helloAgain = factory.CreateLiteralNode("hello");

                Assert.NotEqual(hello, helloEn, "Identical Literals with differing Language Tags are non-equal");
                Assert.NotEqual(hello, helloEnUS, "Identical Literals with differing Language Tags are non-equal");
                Assert.NotEqual(helloEn, helloEnUS, "Identical Literals with differing Language Tags are non-equal");
                Assert.NotEqual(helloEn, helloAgain, "Identical Literals with differing Language Tags are non-equal");
                Assert.NotEqual(helloEnUS, helloAgain, "Identical Literals with differing Language Tags are non-equal");

                Assert.Equal(hello, helloAgain, "Identical Literals with no Language Tag are equal");

                //Test Plain Literals
                INode plain1, plain2, plain3, plain4;
                plain1 = factory.CreateLiteralNode("plain literal");
                plain2 = factory.CreateLiteralNode("another plain literal");
                plain3 = factory.CreateLiteralNode("Plain Literal");
                plain4 = factory.CreateLiteralNode("plain literal");

                Assert.NotEqual(plain1, plain2, "Literals with non-identical lexical values are non-equal");
                Assert.NotEqual(plain1, plain3, "Literals with non-identical lexical values are non-equal even if they differ only in case");
                Assert.Equal(plain1, plain4, "Literals with identical lexical values are equal");
                Assert.NotEqual(plain2, plain3, "Literals with non-identical lexical values are non-equal");
                Assert.NotEqual(plain2, plain4, "Literals with non-identical lexical values are non-equal");
                Assert.NotEqual(plain3, plain4, "Literals with non-identical lexical values are non-equal even if they differ only in case");

                //Typed Literals
                Uri intType = new Uri(XmlSpecsHelper.XmlSchemaDataTypeInteger);
                Uri boolType = new Uri(XmlSpecsHelper.XmlSchemaDataTypeBoolean);

                INode one1, one2, one3, one4;
                one1 = factory.CreateLiteralNode("1");
                one2 = factory.CreateLiteralNode("1", intType);
                one3 = factory.CreateLiteralNode("0001", intType);
                one4 = factory.CreateLiteralNode("1", intType);

                Assert.NotEqual(one1, one2, "Literals with identical lexical values but non-identical data types are non-equal");
                Assert.NotEqual(one1, one3, "Literals with identical lexical values but non-identical data types are non-equal");
                Assert.NotEqual(one1, one4, "Literals with identical lexical values but non-identical data types are non-equal");
                Assert.NotEqual(one2, one3, "Literals with equivalent values represented as different lexical values are non-equal even when they're data types are equal");
                Assert.Equal(one2, one4, "Literals with identical lexical values and identical data types are equal");
                Assert.NotEqual(one3, one4, "Literals with equivalent values represented as different lexical values are non-equal even when they're data types are equal");

                Assert.NotEqual(0, one1.CompareTo(one2), "Using the Comparer for Literal Nodes which is used for sorting Literals with identical lexical values but non-identical data types are still non-equal");
                Assert.Equal(0, one2.CompareTo(one3), "Using the Comparer for Literal Nodes which is used for sorting Literals with equivalent non-identical lexical values are considered equal when their data types are equal");
                Assert.Equal(0, one3.CompareTo(one2), "Using the Comparer for Literal Nodes which is used for sorting Literals with equivalent non-identical lexical values are considered equal when their data types are equal");
                Assert.Equal(0, one3.CompareTo(one4), "Using the Comparer for Literal Nodes which is used for sorting Literals with equivalent non-identical lexical values are considered equal when their data types are equal");

                INode t, f, one5;
                t = factory.CreateLiteralNode("true", boolType);
                f = factory.CreateLiteralNode("false", boolType);
                one5 = factory.CreateLiteralNode("1", boolType);

                Assert.NotEqual(t, f, "Literals with different lexical values but identical data types are non-equal");
                Assert.Equal(t, t, "Literals with identical lexical values and identical data types are equal");
                Assert.Equal(f, f, "Literals with identical lexical values and identical data types are equal");

                Assert.NotEqual(t, one5, "Literals with different data types are non-equal even if their lexical values when cast to that type may be equivalent");

                //Loose Mode Tests
                Console.WriteLine("Doing a load of Loose Equality Tests");
                Options.LiteralEqualityMode = LiteralEqualityMode.Loose;

                Assert.Equal(one2, one3, "Literals with equivalent lexical values and identical data types can be considered equal under Loose Equality Mode");
                Assert.Equal(one3, one4, "Literals with equivalent lexical values and identical data types can be considered equal under Loose Equality Mode");
                Assert.NotEqual(t, one5, "Literals with equivalent lexical values (but which are not in the recognized lexical space of the type i.e. require a cast) and identical data types are still non-equal under Loose Equality Mode");
            }
            finally
            {
                //Reset Literal Equality Mode
                Options.LiteralEqualityMode = LiteralEqualityMode.Strict;
            }
        }

        [Fact]
        public void NodeFactoryContractNodeSorting()
        {
            //Stream for Output
            Console.WriteLine("## Sorting Test");
            Console.WriteLine("NULLs < Blank Nodes < URI Nodes < Untyped Literals < Typed Literals");
            Console.WriteLine();

            //Create a factory
            INodeFactory factory = this.CreateFactoryInstance();

            // Create Nodes
            INode blankA = factory.CreateBlankNode();
            INode blankB = factory.CreateBlankNode();
            INode uriA = factory.CreateUriNode(this.ResolveQName(":a"));
            INode uriA2 = factory.CreateUriNode(uriA.Uri);
            INode uriB = factory.CreateUriNode(this.ResolveQName(":b"));
            INode litImplicit = factory.CreateLiteralNode("value");
            INode litExplicit = factory.CreateLiteralNode("value", this.ResolveQName("xsd:string"));
            INode litLangA = factory.CreateLiteralNode("value", "en");
            INode litLangB = factory.CreateLiteralNode("value", "es");
            INode litInt = factory.CreateLiteralNode("1", this.ResolveQName("xsd:integer"));
            INode litLong = factory.CreateLiteralNode("1", this.ResolveQName("xsd:long"));

            // Test ordering

            // Check things sort equal/not equal as relevant
            CheckSortEqual(blankA, blankA);
            CheckSortEqual(blankB, blankB);
            CheckSortNotEqual(blankA, blankB);
            CheckSortEqual(uriA, uriA);
            CheckSortEqual(uriA, uriA2);
            CheckSortNotEqual(uriA, uriB);
            CheckSortEqual(uriB, uriB);

            // Check relative sort ordering
            INode[] blanks = { blankA, blankB };
            INode[] uris = { uriA, uriA2, uriB };
            INode[] literals = { litImplicit, litExplicit, litLangA, litLangB, litInt, litLong };
            INode[] langLiterals = { litLangA, litLangB };
            INode[] typedLiterals = { litImplicit, litExplicit, litInt, litLong };
            INode[] greaterThanBlanks = { uriA, uriA2, uriB, litImplicit, litExplicit, litLangA, litLangB, litInt, litLong };
            INode[] greaterThanUris = { litImplicit, litExplicit, litLangA, litLangB, litInt, litLong };

            // Blanks < everything else
            CheckSortAscending(blankA, greaterThanBlanks);
            CheckSortAscending(blankB, greaterThanBlanks);
            // URIs sorted in codepoint order
            CheckSortAscending(uriA, uriB);
            CheckSortDescending(uriB, uriA);
            // URIs > blanks and < everything else
            CheckSortAscending(uriA, greaterThanUris);
            CheckSortAscending(uriB, greaterThanUris);
            // Literals sorted by classification (untyped, language, typed) and then codepoint/value order as appropriate
            CheckSortAscending(litLangA, typedLiterals);
            CheckSortAscending(litLangB, typedLiterals);
            CheckSortAscending(litLangA, litLangB);
            CheckSortDescending(litLangB, litLangA);
            CheckSortEqual(litImplicit, litExplicit);
            CheckSortAscending(litInt, litLong);
        }

        protected void CheckSortAscending(INode x, INode y)
        {
            Assert.True(x.CompareTo(y) < 0);
            Assert.True(y.CompareTo(x) > 0);
        }

        protected void CheckSortAscending(INode x, params INode[] ys)
        {
            foreach (INode y in ys)
            {
                CheckSortAscending(x, y);
            }
        }

        protected void CheckSortDescending(INode x, INode y)
        {
            Assert.True(x.CompareTo(y) > 0);
            Assert.True(y.CompareTo(x) < 0);
        }

        protected void CheckSortDescending(INode x, params INode[] ys)
        {
            foreach (INode y in ys)
            {
                CheckSortDescending(x, y);
            }
        }

        protected void CheckSortEqual (INode x, INode y)
        {
            Assert.Equal(0, x.CompareTo(y));
            Assert.Equal(0, y.CompareTo(x));
            Assert.Equal(x, y);
            Assert.Equal(y, x);
        }

        protected void CheckSortNotEqual(INode x, INode y)
        {
            Assert.NotEqual(0, x.CompareTo(y));
            Assert.NotEqual(0, y.CompareTo(x));
            Assert.NotEqual(x, y);
            Assert.NotEqual(y, x);
        }


        [Fact]
        public void NodeFactoryContractNullNodeEquality()
        {
            const UriNode nullUri = null;
            const LiteralNode nullLiteral = null;
            const BlankNode nullBNode = null;

            Graph g = new Graph();
            INode someUri = g.CreateUriNode(new Uri("http://example.org"));
            INode someLiteral = g.CreateLiteralNode("A Literal");
            INode someBNode = g.CreateBlankNode();

            Assert.Equal(nullUri, nullUri, "Null URI Node should be equal to self");
            Assert.Equal(nullUri, null, "Null URI Node should be equal to a null");
            Assert.Equal(null, nullUri, "Null should be equal to a Null URI Node");
            Assert.NotEqual(nullUri, someUri, "Null URI Node should not be equal to an actual URI Node");
            Assert.NotEqual(someUri, nullUri, "Null URI Node should not be equal to an actual URI Node");

            Assert.Equal(nullLiteral, nullLiteral, "Null Literal Node should be equal to self");
            Assert.Equal(nullLiteral, null, "Null Literal Node should be equal to a null");
            Assert.Equal(null, nullLiteral, "Null should be equal to a Null Literal Node");
            Assert.NotEqual(nullLiteral, someLiteral, "Null Literal Node should not be equal to an actual Literal Node");
            Assert.NotEqual(someLiteral, nullLiteral, "Null Literal Node should not be equal to an actual Literal Node");

            Assert.Equal(nullBNode, nullBNode, "Null BNode Node should be equal to self");
            Assert.Equal(nullBNode, null, "Null BNode Node should be equal to a null");
            Assert.Equal(null, nullBNode, "Null should be equal to a Null BNode Node");
            Assert.NotEqual(nullBNode, someBNode, "Null BNode Node should not be equal to an actual BNode Node");
            Assert.NotEqual(someBNode, nullBNode, "Null BNode Node should not be equal to an actual BNode Node");
        }
    }
}