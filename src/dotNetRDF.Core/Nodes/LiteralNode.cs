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
using System.Text;
using VDS.RDF.Specifications;

namespace VDS.RDF.Nodes
{
    /// <summary>
    /// Abstract Base Class for Literal Nodes
    /// </summary>
    public abstract class BaseLiteralNode 
        : BaseNode, IEquatable<BaseLiteralNode>, IComparable<BaseLiteralNode>
    {
        /// <summary>
        /// Internal Only Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        protected internal BaseLiteralNode(String literal)
            : this(literal, Options.LiteralValueNormalization) { }

        /// <summary>
        /// Internal Only Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="normalize">Whether to Normalize the Literal Value</param>
        protected internal BaseLiteralNode(String literal, bool normalize)
            : this(literal, null, null, normalize) { }

        /// <summary>
        /// Internal Only Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="langspec">String value for the Language Specifier for the Literal</param>
        protected internal BaseLiteralNode(String literal, String langspec)
            : this(literal, langspec, Options.LiteralValueNormalization) { }

        /// <summary>
        /// Internal Only Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="langspec">String value for the Language Specifier for the Literal</param>
        /// <param name="normalize">Whether to Normalize the Literal Value</param>
        protected internal BaseLiteralNode(String literal, String langspec, bool normalize)
            : this(literal, langspec, null, normalize) { }

        /// <summary>
        /// Internal Only Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="datatype">Uri for the Literals Data Type</param>
        protected internal BaseLiteralNode(String literal, Uri datatype)
            : this(literal, null, datatype, Options.LiteralValueNormalization) { }

        /// <summary>
        /// Internal Only Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="langspec">Language specifier for the Literal</param>
        /// <param name="datatype">Uri for the Literals Data Type</param>
        /// <param name="normalize">Whether to Normalize the Literal Value</param>
        protected internal BaseLiteralNode(String literal, String langspec, Uri datatype, bool normalize)
            : base(NodeType.Literal)
        {
            // Normalize value if appropriate
            if (normalize)
            {
                // TODO: Currently no support in .NET Core for String.Normalize() - may need to find a way around this
//#if !NO_NORM
//                this.Value = literal != null ? literal.Normalize() : String.Empty;
//#else
                this.Value = literal != null ? literal : String.Empty;
//#endif
            }
            else
            {
                this.Value = literal ?? String.Empty;
            }

            // Remember to lower case language specifiers
            this.Language = String.IsNullOrEmpty(langspec) ? null : langspec.ToLowerInvariant();

            if (Options.Rdf11)
            {
                // If RDF 1.1 use rdf:langString/xsd:string or explicit type whichever is most appropriate based on the other arguments
                this.DataType = this.HasLanguage ? UriFactory.Create(RdfSpecsHelper.RdfLangString) : (datatype ?? UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeString));
            }
            else
            {
                // Otherwise just use datatype given
                this.DataType = datatype;
            }

            //Compute Hash Code
            this._hashcode = Tools.CreateHashCode(this);
        }

        /// <summary>
        /// Gives the lexical value of the literal
        /// </summary>
        public override String Value { get; protected set; }

        /// <summary>
        /// Gets whether the literal has a language specifier
        /// </summary>
        public override bool HasLanguage
        {
            get { return !String.IsNullOrEmpty(this.Language); }
        }

        /// <summary>
        /// Gives the language specifier for the literal (if it exists) or null
        /// </summary>
        /// <remarks>
        /// Note that with RDF 1.1 both this and <see cref="HasDataType"/> may return true, you should always test for this before testing for <see cref="HasDataType"/>
        /// </remarks>
        public override String Language { get; protected set; }

        /// <summary>
        /// Gets whether the literal has a data type URI
        /// </summary>
        /// <remarks>
        /// Note that with RDF 1.1 both this and <see cref="HasLanguage"/> may return true, you should always test for <see cref="HasLanguage"/> before testing for this.
        /// </remarks>
        public override bool HasDataType
        {
            get { return !ReferenceEquals(this.DataType, null); }
        }

        /// <summary>
        /// Gives the data type URI for the literal (if it exists) or null
        /// </summary>
        public override Uri DataType { get; protected set; }

        /// <summary>
        /// Implementation of the Equals method for Literal Nodes
        /// </summary>
        /// <param name="obj">Object to compare the Node with</param>
        /// <returns></returns>
        /// <remarks>
        /// The default behaviour is for Literal Nodes to be considered equal IFF
        /// <ol>
        /// <li>Their Language Specifiers are identical (or neither has a Language Specifier)</li>
        /// <li>Their Data Types are identical (or neither has a Data Type)</li>
        /// <li>Their String values are identical</li>
        /// </ol>
        /// This behaviour can be overridden to use value equality by setting the <see cref="Options.LiteralEqualityMode">LiteralEqualityMode</see> option to be <see cref="LiteralEqualityMode.Loose">Loose</see> if this is more suited to your application.
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj is INode)
            {
                return this.Equals((INode)obj);
            }
            //Can only be equal to other Nodes
            return false;
        }

        /// <summary>
        /// Implementation of the Equals method for Literal Nodes
        /// </summary>
        /// <param name="other">Object to compare the Node with</param>
        /// <returns></returns>
        /// <remarks>
        /// The default behaviour is for Literal Nodes to be considered equal IFF
        /// <ol>
        /// <li>Their Language Specifiers are identical (or neither has a Language Specifier)</li>
        /// <li>Their Data Types are identical (or neither has a Data Type)</li>
        /// <li>Their String values are identical</li>
        /// </ol>
        /// This behaviour can be overridden to use value equality by setting the <see cref="Options.LiteralEqualityMode">LiteralEqualityMode</see> option to be <see cref="LiteralEqualityMode.Loose">Loose</see> if this is more suited to your application.
        /// </remarks>
        public override bool Equals(INode other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            return other.NodeType == NodeType.Literal && EqualityHelper.AreLiteralsEqual(this, other);
        }

        /// <summary>
        /// Determines whether this Node is equal to a Literal Node
        /// </summary>
        /// <param name="other">Literal Node</param>
        /// <returns></returns>
        public bool Equals(BaseLiteralNode other)
        {
            return this.Equals((INode)other);
        }

        /// <summary>
        /// Gets a String representation of a Literal Node
        /// </summary>
        /// <returns></returns>
        /// <remarks>Gives a value without quotes (as some syntaxes use) with the Data Type/Language Specifier appended using Notation 3 syntax</remarks>
        public override string ToString()
        {
            StringBuilder stringOut = new StringBuilder();
            stringOut.Append(this.Value);
            if (this.HasLanguage)
            {
                stringOut.Append("@");
                stringOut.Append(this.Language);
            }
            else if (this.HasDataType)
            {
                stringOut.Append("^^");
                stringOut.Append(this.DataType.AbsoluteUri);
            }

            return stringOut.ToString();
        }

        /// <summary>
        /// Implementation of CompareTo for Literal Nodes
        /// </summary>
        /// <param name="other">Node to Compare To</param>
        /// <returns></returns>
        /// <remarks>
        /// Literal Nodes are greater than Blank Nodes, Uri Nodes and Nulls, they are less than Graph Literal Nodes.
        /// <br /><br />
        /// Two Literal Nodes are initially compared based upon Data Type, untyped literals are less than typed literals.  Two untyped literals are compared purely on lexical value, Language Specifier has no effect on the ordering.  This means Literal Nodes are only partially ordered, for example "hello"@en and "hello"@en-us are considered to be the same for ordering purposes though they are different for equality purposes.  Datatyped Literals can only be properly ordered if they are one of a small subset of types (Integers, Booleans, Date Times, Strings and URIs).  If the datatypes for two Literals are non-matching they are ordered on Datatype Uri, this ensures that each range of Literal Nodes is sorted to some degree.  Again this also means that Literals are partially ordered since unknown datatypes will only be sorted based on lexical value and not on actual value.
        /// </remarks>
        public override int CompareTo(INode other)
        {
            if (ReferenceEquals(this, other)) return 0;

            if (ReferenceEquals(other, null))
            {
                //Everything is greater than a null
                //Return a 1 to indicate this
                return 1;
            }
            if (other.NodeType == NodeType.Blank || other.NodeType == NodeType.Variable || other.NodeType == NodeType.Uri)
            {
                //Literal Nodes are greater than Blank, Variable and Uri Nodes
                //Return a 1 to indicate this
                return 1;
            }
            if (other.NodeType == NodeType.Literal)
            {
                return ComparisonHelper.CompareLiterals(this, other);
            }
            //Anything else is considered greater than a Literal Node
            //Return -1 to indicate this
            return -1;
        }

        /// <summary>
        /// Returns an Integer indicating the Ordering of this Node compared to another Node
        /// </summary>
        /// <param name="other">Node to test against</param>
        /// <returns></returns>
        public int CompareTo(BaseLiteralNode other)
        {
            return this.CompareTo((INode)other);
        }

    }

    /// <summary>
    /// Class for representing Literal Nodes
    /// </summary>
    public class LiteralNode
        : BaseLiteralNode, IEquatable<LiteralNode>, IComparable<LiteralNode>
    {
        /// <summary>
        /// Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        public LiteralNode(String literal)
            : this(literal, Options.LiteralValueNormalization) { }

        /// <summary>
        /// Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="normalize">Whether to Normalize the Literal Value</param>
        public LiteralNode(String literal, bool normalize)
            : base(literal, normalize) { }

        /// <summary>
        /// Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="langspec">String value for the Language Specifier for the Literal</param>
        public LiteralNode(String literal, String langspec)
            : this(literal, langspec, Options.LiteralValueNormalization) { }

        /// <summary>
        /// Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="langspec">String value for the Language Specifier for the Literal</param>
        /// <param name="normalize">Whether to Normalize the Literal Value</param>
        public LiteralNode(String literal, String langspec, bool normalize)
            : base(literal, langspec, normalize) { }

        /// <summary>
        /// Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="datatype">Uri for the Literals Data Type</param>
        public LiteralNode(String literal, Uri datatype)
            : this(literal, datatype, Options.LiteralValueNormalization) { }

        /// <summary>
        /// Constructor for Literal Nodes
        /// </summary>
        /// <param name="g">Graph this Node is in</param>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="datatype">Uri for the Literals Data Type</param>
        /// <param name="normalize">Whether to Normalize the Literal Value</param>
        public LiteralNode(String literal, Uri datatype, bool normalize)
            : base(literal, null, datatype, normalize) { }

        /// <summary>
        /// Implementation of Compare To for Literal Nodes
        /// </summary>
        /// <param name="other">Literal Node to Compare To</param>
        /// <returns></returns>
        /// <remarks>
        /// Simply invokes the more general implementation of this method
        /// </remarks>
        public int CompareTo(LiteralNode other)
        {
            return this.CompareTo((INode)other);
        }

        /// <summary>
        /// Determines whether this Node is equal to a Literal Node
        /// </summary>
        /// <param name="other">Literal Node</param>
        /// <returns></returns>
        public bool Equals(LiteralNode other)
        {
            return base.Equals((INode)other);
        }
    }

    /// <summary>
    /// Class for representing Literal Nodes where the Literal values are not normalized
    /// </summary>
    class NonNormalizedLiteralNode 
        : LiteralNode, IComparable<NonNormalizedLiteralNode>
    {
        /// <summary>
        /// Internal Only Constructor for Literal Nodes
        /// </summary>
        /// <param name="literal">String value of the Literal</param>
        protected internal NonNormalizedLiteralNode(String literal)
            : base(literal, false) { }

        /// <summary>
        /// Internal Only Constructor for Literal Nodes
        /// </summary>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="langspec">Lanaguage Specifier for the Literal</param>
        protected internal NonNormalizedLiteralNode(String literal, String langspec)
            : base(literal, langspec, false) { }

        /// <summary>
        /// Internal Only Constructor for Literal Nodes
        /// </summary>
        /// <param name="literal">String value of the Literal</param>
        /// <param name="datatype">Uri for the Literals Data Type</param>
        protected internal NonNormalizedLiteralNode(String literal, Uri datatype)
            : base(literal, datatype, false) { }

        /// <summary>
        /// Implementation of Compare To for Literal Nodes
        /// </summary>
        /// <param name="other">Literal Node to Compare To</param>
        /// <returns></returns>
        /// <remarks>
        /// Simply invokes the more general implementation of this method
        /// </remarks>
        public int CompareTo(NonNormalizedLiteralNode other)
        {
            return this.CompareTo((INode)other);
        }
    }
}
