using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using VDS.RDF.Graphs;
using VDS.RDF.Nodes;
using VDS.RDF.Query.Algebra;
using VDS.RDF.Query.Elements;
using VDS.RDF.Query.Expressions;
using VDS.RDF.Query.Expressions.Aggregates;
using VDS.RDF.Query.Expressions.Aggregates.Sparql;
using VDS.RDF.Query.Expressions.Comparison;
using VDS.RDF.Query.Expressions.Primary;
using VDS.RDF.Query.Paths;
using VDS.RDF.Query.Results;

namespace VDS.RDF.Query.Compiler
{
    public abstract class AbstractQueryCompilerTests
    {
        protected INodeFactory NodeFactory { get; set; }

        public AbstractQueryCompilerTests()
        {
            if (this.NodeFactory == null) this.NodeFactory = new NodeFactory();
        }

        /// <summary>
        /// Creates a new query compiler instance to use for testing
        /// </summary>
        /// <returns></returns>
        protected abstract IQueryCompiler CreateInstance();

        [Fact]
        public void QueryCompilerEmptyWhere()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Table), algebra);

            Table table = (Table) algebra;
            Assert.True(table.IsUnit);
        }

        [Fact]
        public void QueryCompilerEmptyBgp()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            query.WhereClause = new TripleBlockElement(Enumerable.Empty<Triple>());

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Table), algebra);

            Table table = (Table) algebra;
            Assert.True(table.IsUnit);
        }

        [Fact]
        public void QueryCompilerBgp()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            query.WhereClause = new TripleBlockElement(t.AsEnumerable());

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Bgp), algebra);

            Bgp bgp = (Bgp) algebra;
            Assert.Equal(1, bgp.TriplePatterns.Count);
            Assert.True(bgp.TriplePatterns.Contains(t));
        }

        [Fact]
        public void QueryCompilerEmptyPathBlock()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            query.WhereClause = new PathBlockElement(Enumerable.Empty<TriplePath>());

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof(Table), algebra);

            Table table = (Table)algebra;
            Assert.True(table.IsUnit);
        }

        [Fact]
        public void QueryCompilerPathBlock1()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            query.WhereClause = new PathBlockElement(new TriplePath(t).AsEnumerable());

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof(Bgp), algebra);

            Bgp bgp = (Bgp)algebra;
            Assert.Equal(1, bgp.TriplePatterns.Count);
            Assert.True(bgp.TriplePatterns.Contains(t));
        }

        [Fact]
        public void QueryCompilerPathBlock2()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            query.WhereClause = new PathBlockElement(new TriplePath[] { new TriplePath(t), new TriplePath(t.Subject, new InversePath(new Property(t.Predicate)), t.Object) });

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof(PropertyPath), algebra);

            PropertyPath pp = (PropertyPath) algebra;
            Assert.True(pp.TriplePath.IsPath);
            Assert.IsType(typeof(InversePath), pp.TriplePath.Path);
            Assert.Equal(t.Subject, pp.TriplePath.Subject);
            Assert.Equal(t.Object, pp.TriplePath.Object);

            Assert.IsType(typeof(Bgp), pp.InnerAlgebra);
            Bgp bgp = (Bgp)pp.InnerAlgebra;
            Assert.Equal(1, bgp.TriplePatterns.Count);
            Assert.True(bgp.TriplePatterns.Contains(t));
        }

        [Fact]
        public void QueryCompilerPathBlock3()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            INode seqA = new UriNode(new Uri("http://a"));
            INode seqB = new UriNode(new Uri("http://b"));
            query.WhereClause = new PathBlockElement(new TriplePath[] { new TriplePath(t), new TriplePath(t.Subject, new InversePath(new Property(t.Predicate)), t.Object), new TriplePath(t.Subject, new SequencePath(new Property(seqA), new Property(seqB)), t.Object) });

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof(PropertyPath), algebra);

            PropertyPath pp = (PropertyPath)algebra;
            Assert.True(pp.TriplePath.IsPath);
            Assert.IsType(typeof(SequencePath), pp.TriplePath.Path);
            Assert.Equal(t.Subject, pp.TriplePath.Subject);
            Assert.Equal(t.Object, pp.TriplePath.Object);

            Assert.IsType(typeof(PropertyPath), pp.InnerAlgebra);
            pp = (PropertyPath) pp.InnerAlgebra;
            Assert.IsType(typeof(InversePath), pp.TriplePath.Path);
            Assert.Equal(t.Subject, pp.TriplePath.Subject);
            Assert.Equal(t.Object, pp.TriplePath.Object);

            Assert.IsType(typeof(Bgp), pp.InnerAlgebra);
            Bgp bgp = (Bgp)pp.InnerAlgebra;
            Assert.Equal(1, bgp.TriplePatterns.Count);
            Assert.True(bgp.TriplePatterns.Contains(t));
        }

        [Fact]
        public void QueryCompilerUnion1()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            IElement triples = new TripleBlockElement(t.AsEnumerable());

            IElement union = new UnionElement(triples.AsEnumerable().Concat(triples.AsEnumerable()));
            query.WhereClause = union;

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Union), algebra);

            Union u = (Union) algebra;
            Assert.IsType(typeof (Bgp), u.Lhs);
            Assert.IsType(typeof (Bgp), u.Rhs);
        }

        [Fact]
        public void QueryCompilerUnion2()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t1 = new Triple(new VariableNode("a"), new VariableNode("b"), new VariableNode("c"));
            Triple t2 = new Triple(new VariableNode("d"), new VariableNode("e"), new VariableNode("f"));
            Triple t3 = new Triple(new VariableNode("g"), new VariableNode("h"), new VariableNode("i"));
            IElement triples1 = new TripleBlockElement(t1.AsEnumerable());
            IElement triples2 = new TripleBlockElement(t2.AsEnumerable());
            IElement triples3 = new TripleBlockElement(t3.AsEnumerable());
            IElement[] elements = {triples1, triples2, triples3};

            IElement union = new UnionElement(elements);
            query.WhereClause = union;

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Union), algebra);

            Union u = (Union) algebra;
            Assert.IsType(typeof (Bgp), u.Lhs);

            Bgp bgp = (Bgp) u.Lhs;
            Assert.Equal(1, bgp.TriplePatterns.Count);
            Assert.True(bgp.TriplePatterns.Contains(t1));

            Assert.IsType(typeof (Union), u.Rhs);
            u = (Union) u.Rhs;
            Assert.IsType(typeof (Bgp), u.Lhs);

            bgp = (Bgp) u.Lhs;
            Assert.Equal(1, bgp.TriplePatterns.Count);
            Assert.True(bgp.TriplePatterns.Contains(t2));

            bgp = (Bgp) u.Rhs;
            Assert.Equal(1, bgp.TriplePatterns.Count);
            Assert.True(bgp.TriplePatterns.Contains(t3));
        }

        [Fact]
        public void QueryCompilerInlineEmptyValues()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            IMutableTabularResults data = new MutableTabularResults(Enumerable.Empty<String>(), Enumerable.Empty<IMutableResultRow>());
            query.WhereClause = new DataElement(data);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Table), algebra);

            Table table = (Table) algebra;
            Assert.True(table.IsEmpty);
        }

        [Fact]
        public void QueryCompilerInlineValues1()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            IMutableTabularResults data = new MutableTabularResults("x".AsEnumerable(), Enumerable.Empty<IMutableResultRow>());
            data.Add(new MutableResultRow("x".AsEnumerable(), new Dictionary<string, INode> {{"x", 1.ToLiteral(this.NodeFactory)}}));
            query.WhereClause = new DataElement(data);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Table), algebra);

            Table table = (Table) algebra;
            Assert.False(table.IsEmpty);
            Assert.False(table.IsUnit);

            Assert.Equal(1, table.Data.Count);
            Assert.True(table.Data.All(s => s.ContainsVariable("x")));
        }

        [Fact]
        public void QueryCompilerInlineValues2()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            IMutableTabularResults data = new MutableTabularResults(new String[] {"x", "y"}, Enumerable.Empty<IMutableResultRow>());
            data.Add(new MutableResultRow("x".AsEnumerable(), new Dictionary<string, INode> {{"x", 1.ToLiteral(this.NodeFactory)}}));
            data.Add(new MutableResultRow("y".AsEnumerable(), new Dictionary<string, INode> {{"y", 2.ToLiteral(this.NodeFactory)}}));
            data.Add(new MutableResultRow(data.Variables, new Dictionary<string, INode> {{"x", 3.ToLiteral(this.NodeFactory)}, {"y", 4.ToLiteral(this.NodeFactory)}}));
            query.WhereClause = new DataElement(data);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Table), algebra);

            Table table = (Table) algebra;
            Assert.False(table.IsEmpty);
            Assert.False(table.IsUnit);

            Assert.Equal(3, table.Data.Count);
            Assert.True(table.Data.All(s => s.ContainsVariable("x") || s.ContainsVariable("y")));
            Assert.False(table.Data.All(s => s.ContainsVariable("x") && s.ContainsVariable("y")));
            Assert.True(table.Data.Any(s => s.ContainsVariable("x") && s.ContainsVariable("y")));
        }

        [Fact]
        public void QueryCompilerValues1()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            IMutableTabularResults data = new MutableTabularResults("x".AsEnumerable(), Enumerable.Empty<IMutableResultRow>());
            data.Add(new MutableResultRow("x".AsEnumerable(), new Dictionary<string, INode> {{"x", 1.ToLiteral(this.NodeFactory)}}));
            query.ValuesClause = data;

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Table), algebra);

            Table table = (Table) algebra;
            Assert.False(table.IsEmpty);
            Assert.False(table.IsUnit);

            Assert.Equal(1, table.Data.Count);
            Assert.True(table.Data.All(s => s.ContainsVariable("x")));
        }

        [Fact]
        public void QueryCompilerValues2()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            IMutableTabularResults data = new MutableTabularResults(new String[] {"x", "y"}, Enumerable.Empty<IMutableResultRow>());
            data.Add(new MutableResultRow("x".AsEnumerable(), new Dictionary<string, INode> {{"x", 1.ToLiteral(this.NodeFactory)}}));
            data.Add(new MutableResultRow("y".AsEnumerable(), new Dictionary<string, INode> {{"y", 2.ToLiteral(this.NodeFactory)}}));
            data.Add(new MutableResultRow(data.Variables, new Dictionary<string, INode> {{"x", 3.ToLiteral(this.NodeFactory)}, {"y", 4.ToLiteral(this.NodeFactory)}}));
            query.ValuesClause = data;

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Table), algebra);

            Table table = (Table) algebra;
            Assert.False(table.IsEmpty);
            Assert.False(table.IsUnit);

            Assert.Equal(3, table.Data.Count);
            Assert.True(table.Data.All(s => s.ContainsVariable("x") || s.ContainsVariable("y")));
            Assert.False(table.Data.All(s => s.ContainsVariable("x") && s.ContainsVariable("y")));
            Assert.True(table.Data.Any(s => s.ContainsVariable("x") && s.ContainsVariable("y")));
        }

        [Fact]
        public void QueryCompilerEmptyValues()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            IMutableTabularResults data = new MutableTabularResults(Enumerable.Empty<String>(), Enumerable.Empty<IMutableResultRow>());
            query.ValuesClause = data;

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Table), algebra);

            Table table = (Table) algebra;
            Assert.True(table.IsEmpty);
        }

        [Theory]
        [InlineData(0),
         InlineData(100),
         InlineData(Int64.MaxValue),
         InlineData(-1),
         InlineData(Int64.MinValue)]
        public void QueryCompilerLimit(long limit)
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            query.Limit = limit;
            Assert.True(limit >= 0L ? query.HasLimit : !query.HasLimit);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            if (limit >= 0L)
            {
                Assert.IsType(typeof (Slice), algebra);

                Slice slice = (Slice) algebra;
                Assert.Equal(limit, slice.Limit);
                Assert.Equal(0L, slice.Offset);
            }
            else
            {
                Assert.IsType(typeof (Table), algebra);

                Table table = (Table) algebra;
                Assert.True(table.IsUnit);
            }
        }

        [Theory, InlineData(0),
         InlineData(100),
         InlineData(Int64.MaxValue),
         InlineData(-1),
         InlineData(Int64.MinValue)]
        public void QueryCompilerOffset(long offset)
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            query.Offset = offset;
            Assert.True(offset > 0L ? query.HasOffset : !query.HasOffset);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            if (offset > 0L)
            {
                Assert.IsType(typeof (Slice), algebra);

                Slice slice = (Slice) algebra;
                Assert.Equal(offset, slice.Offset);
                Assert.Equal(-1L, slice.Limit);
            }
            else
            {
                Assert.IsType(typeof (Table), algebra);

                Table table = (Table) algebra;
                Assert.True(table.IsUnit);
            }
        }

        [Theory, InlineData(0, 0),
         InlineData(100, 0),
         InlineData(100, 5000),
         InlineData(Int64.MaxValue, 0),
         InlineData(0, Int64.MaxValue),
         InlineData(-1, -1),
         InlineData(-1, 100),
         InlineData(Int64.MinValue, 0),
         InlineData(0, Int64.MinValue)]
        public void QueryCompilerLimitOffset(long limit, long offset)
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            query.Limit = limit;
            query.Offset = offset;
            Assert.True(limit >= 0L ? query.HasLimit : !query.HasLimit);
            Assert.True(offset > 0L ? query.HasOffset : !query.HasOffset);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            if (limit >= 0L || offset > 0L)
            {
                Assert.IsType(typeof (Slice), algebra);

                Slice slice = (Slice) algebra;
                Assert.Equal(limit >= 0L ? limit : -1L, slice.Limit);
                Assert.Equal(offset > 0L ? offset : 0L, slice.Offset);
            }
            else
            {
                Assert.IsType(typeof (Table), algebra);

                Table table = (Table) algebra;
                Assert.True(table.IsUnit);
            }
        }

        [Theory, InlineData(QueryType.SelectAllDistinct),
         InlineData(QueryType.SelectDistinct)]
        public void QueryCompilerDistinct(QueryType type)
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = type;

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            Assert.IsType(typeof (Distinct), algebra);
        }

        [Theory, InlineData(QueryType.SelectAllReduced),
         InlineData(QueryType.SelectReduced)]
        public void QueryCompilerReduced(QueryType type)
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = type;

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            Assert.IsType(typeof (Reduced), algebra);
        }

        [Theory, InlineData("http://example.org", false),
         InlineData("http://example.org", true),
         InlineData("http://foo.bar/faz", false)]
        public void QueryCompilerService(String endpoint, bool silent)
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Uri endpointUri = new Uri(endpoint);
            query.WhereClause = new ServiceElement(new TripleBlockElement(), endpointUri, silent);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Service), algebra);

            Service service = (Service) algebra;
            Assert.Equal(silent, service.IsSilent);
            Assert.True(EqualityHelper.AreUrisEqual(endpointUri, service.EndpointUri));
        }

        [Fact]
        public void QueryCompilerBind1()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            IExpression expr = new ConstantTerm(true.ToLiteral(this.NodeFactory));
            query.WhereClause = new BindElement(new KeyValuePair<String, IExpression>("x", expr).AsEnumerable());

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Extend), algebra);

            Extend extend = (Extend) algebra;
            Assert.Equal(1, extend.Assignments.Count);
            Assert.Equal("x", extend.Assignments[0].Key);
            Assert.Equal(expr, extend.Assignments[0].Value);
        }

        [Fact]
        public void QueryCompilerBind2()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            IExpression expr1 = new ConstantTerm(true.ToLiteral(this.NodeFactory));
            IExpression expr2 = new ConstantTerm(false.ToLiteral(this.NodeFactory));
            IElement[] elements =
            {
                new BindElement(new KeyValuePair<String, IExpression>("x", expr1).AsEnumerable()), new BindElement(new KeyValuePair<String, IExpression>("y", expr2).AsEnumerable())
            };
            query.WhereClause = new GroupElement(elements);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Extend), algebra);

            Extend extend = (Extend) algebra;
            Assert.Equal(2, extend.Assignments.Count);
            Assert.Equal("x", extend.Assignments[0].Key);
            Assert.Equal(expr1, extend.Assignments[0].Value);
            Assert.Equal("y", extend.Assignments[1].Key);
            Assert.Equal(expr2, extend.Assignments[1].Value);
        }

        [Fact]
        public void QueryCompilerFilter1()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            IExpression expr = new ConstantTerm(true.ToLiteral(this.NodeFactory));
            query.WhereClause = new FilterElement(expr.AsEnumerable());

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Filter), algebra);

            Filter filter = (Filter) algebra;
            Assert.Equal(1, filter.Expressions.Count);
            Assert.Equal(expr, filter.Expressions[0]);
        }

        [Fact]
        public void QueryCompilerMinus1()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            TripleBlockElement triples = new TripleBlockElement(t.AsEnumerable());
            query.WhereClause = new MinusElement(triples);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof (Minus), algebra);

            Minus minus = (Minus) algebra;
            Assert.IsType(typeof(Table), minus.Lhs);
            Assert.IsType(typeof(Bgp), minus.Rhs);

            Table lhs = (Table) minus.Lhs;
            Assert.True(lhs.IsUnit);

            Bgp rhs = (Bgp) minus.Rhs;
            Assert.Equal(1, rhs.TriplePatterns.Count);
            Assert.True(rhs.TriplePatterns.Contains(t));
        }

        [Fact]
        public void QueryCompilerMinus2()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t1 = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            Triple t2 = new Triple(new VariableNode("s"), new BlankNode(Guid.NewGuid()), new LiteralNode("test"));
            TripleBlockElement matchTriples = new TripleBlockElement(t1.AsEnumerable());
            TripleBlockElement minusTriples = new TripleBlockElement(t2.AsEnumerable());
            query.WhereClause = new GroupElement(new IElement[] { matchTriples, new MinusElement(minusTriples) });

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof(Minus), algebra);

            Minus minus = (Minus)algebra;
            Assert.IsType(typeof(Bgp), minus.Lhs);
            Assert.IsType(typeof(Bgp), minus.Rhs);

            Bgp lhs = (Bgp)minus.Lhs;
            Assert.Equal(1, lhs.TriplePatterns.Count);
            Assert.True(lhs.TriplePatterns.Contains(t1));

            Bgp rhs = (Bgp)minus.Rhs;
            Assert.Equal(1, rhs.TriplePatterns.Count);
            Assert.True(rhs.TriplePatterns.Contains(t2));
        }

        [Fact]
        public void QueryCompilerOptional1()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            TripleBlockElement triples = new TripleBlockElement(t.AsEnumerable());
            query.WhereClause = new OptionalElement(triples);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof(LeftJoin), algebra);

            LeftJoin leftJoin = (LeftJoin)algebra;
            Assert.IsType(typeof(Table), leftJoin.Lhs);
            Assert.IsType(typeof(Bgp), leftJoin.Rhs);

            Table lhs = (Table)leftJoin.Lhs;
            Assert.True(lhs.IsUnit);

            Bgp rhs = (Bgp)leftJoin.Rhs;
            Assert.Equal(1, rhs.TriplePatterns.Count);
            Assert.True(rhs.TriplePatterns.Contains(t));
        }

        [Fact]
        public void QueryCompilerOptional2()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t1 = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            Triple t2 = new Triple(new VariableNode("s"), new BlankNode(Guid.NewGuid()), new LiteralNode("test"));
            TripleBlockElement matchTriples = new TripleBlockElement(t1.AsEnumerable());
            TripleBlockElement minusTriples = new TripleBlockElement(t2.AsEnumerable());
            query.WhereClause = new GroupElement(new IElement[] { matchTriples, new OptionalElement(minusTriples) });

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof(LeftJoin), algebra);

            LeftJoin leftJoin = (LeftJoin)algebra;
            Assert.IsType(typeof(Bgp), leftJoin.Lhs);
            Assert.IsType(typeof(Bgp), leftJoin.Rhs);

            Bgp lhs = (Bgp)leftJoin.Lhs;
            Assert.Equal(1, lhs.TriplePatterns.Count);
            Assert.True(lhs.TriplePatterns.Contains(t1));

            Bgp rhs = (Bgp)leftJoin.Rhs;
            Assert.Equal(1, rhs.TriplePatterns.Count);
            Assert.True(rhs.TriplePatterns.Contains(t2));
        }

        [Fact]
        public void QueryCompilerGroup1()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            IMutableTabularResults data = new MutableTabularResults("x".AsEnumerable(), Enumerable.Empty<IMutableResultRow>());
            data.Add(new MutableResultRow("x".AsEnumerable(), new Dictionary<string, INode> {{"x", 1.ToLiteral(this.NodeFactory)}}));
            TripleBlockElement tripleBlock = new TripleBlockElement(t.AsEnumerable());
            DataElement inlineData = new DataElement(data);
            query.WhereClause = new GroupElement(new IElement[] {tripleBlock, inlineData});

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            Assert.IsType(typeof (Join), algebra);
            Join join = (Join) algebra;

            Assert.IsType(typeof (Bgp), join.Lhs);
            Bgp bgp = (Bgp) join.Lhs;
            Assert.Equal(1, bgp.TriplePatterns.Count);
            Assert.True(bgp.TriplePatterns.Contains(t));

            Assert.IsType(typeof (Table), join.Rhs);
            Table table = (Table) join.Rhs;
            Assert.False(table.IsEmpty);
            Assert.False(table.IsUnit);

            Assert.Equal(1, table.Data.Count);
            Assert.True(table.Data.All(s => s.ContainsVariable("x")));
        }

        [Fact]
        public void QueryCompilerGroup2()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            Triple t = new Triple(new VariableNode("s"), new VariableNode("p"), new VariableNode("o"));
            TripleBlockElement tripleBlock = new TripleBlockElement(t.AsEnumerable());

            IMutableTabularResults data = new MutableTabularResults("x".AsEnumerable(), Enumerable.Empty<IMutableResultRow>());
            data.Add(new MutableResultRow("x".AsEnumerable(), new Dictionary<string, INode> {{"x", 1.ToLiteral(this.NodeFactory)}}));
            DataElement inlineData = new DataElement(data);

            IExpression expr = new ConstantTerm(true.ToLiteral(this.NodeFactory));
            FilterElement filter = new FilterElement(expr.AsEnumerable());

            query.WhereClause = new GroupElement(new IElement[] {tripleBlock, filter, inlineData});

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            Assert.IsType(typeof (Filter), algebra);
            Filter f = (Filter) algebra;
            Assert.Equal(1, f.Expressions.Count);
            Assert.Equal(expr, f.Expressions[0]);

            Assert.IsType(typeof (Join), f.InnerAlgebra);
            Join join = (Join) f.InnerAlgebra;

            Assert.IsType(typeof (Bgp), join.Lhs);
            Bgp bgp = (Bgp) join.Lhs;
            Assert.Equal(1, bgp.TriplePatterns.Count);
            Assert.True(bgp.TriplePatterns.Contains(t));

            Assert.IsType(typeof (Table), join.Rhs);
            Table table = (Table) join.Rhs;
            Assert.False(table.IsEmpty);
            Assert.False(table.IsUnit);

            Assert.Equal(1, table.Data.Count);
            Assert.True(table.Data.All(s => s.ContainsVariable("x")));
        }

        [Fact]
        public void QueryCompilerSubQuery1()
        {
            IQueryCompiler compiler = this.CreateInstance();

            ISparqlQuery query = new SparqlQuery();
            query.AddProjectVariable("y");
            ISparqlQuery subQuery = new SparqlQuery();
            subQuery.AddProjectVariable("x");
            query.WhereClause = new SubQueryElement(subQuery);

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof(Project), algebra);

            Project outerProject = (Project) algebra;
            Assert.Equal(1, outerProject.Projections.Count);
            Assert.True(outerProject.Projections.Contains("y"));
            Assert.IsType(typeof(Project), outerProject.InnerAlgebra);

            Project innerProject = (Project) outerProject.InnerAlgebra;
            Assert.True(innerProject.Projections.Contains("x"));
            Assert.IsType(typeof(Table), innerProject.InnerAlgebra);

            Table table = (Table) innerProject.InnerAlgebra;
            Assert.True(table.IsUnit);
        }

        public static IEnumerable<object[]> TestVars
        {
            get
            {
                return new []
                {
                    new object[] {new string[] {"x"} },
                    new object[] {new string[] { "x", "y"} },
                    new object[] {new string[] { "x", "y", "z"} }
                };
            }
        }

        [Theory, MemberData("TestVars")]
        public void QueryCompilerProject(String[] vars)
        {
            IQueryCompiler compiler = new DefaultQueryCompiler();

            ISparqlQuery query = new SparqlQuery();
            foreach (String var in vars)
            {
                query.AddProjectVariable(var);
            }

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());
            Assert.IsType(typeof(Project), algebra);

            Project project = (Project) algebra;
            Assert.Equal(vars.Length, project.Projections.Count);
            foreach (String var in vars)
            {
                Assert.True(project.Projections.Contains(var), "Project for ?" + var +  " missing");
            }
        }

        [Fact]
        public void QueryCompilerGroupBy1()
        {
            IQueryCompiler compiler = new DefaultQueryCompiler();

            ISparqlQuery query = new SparqlQuery();
            query.AddProjectExpression("x", new CountAllAggregate());

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            Assert.IsType(typeof(Project), algebra);
            Project project = (Project)algebra;
            Assert.Equal(1, project.Projections.Count);
            Assert.True(project.Projections.Contains("x"));

            Assert.IsType(typeof(Extend), project.InnerAlgebra);
            Extend extend = (Extend) project.InnerAlgebra;
            Assert.Equal(1, extend.Assignments.Count);
            Assert.Equal(query.Projections.First().Key, extend.Assignments.First().Key);

            Assert.IsType(typeof(GroupBy), extend.InnerAlgebra);
            GroupBy group = (GroupBy) extend.InnerAlgebra;
            Assert.Equal(0, group.GroupExpressions.Count);
            Assert.Equal(1, group.Aggregators.Count);
            Assert.Equal(query.Projections.First().Value, group.Aggregators.First().Key);
        }

        [Fact]
        public void QueryCompilerGroupBy2()
        {
            IQueryCompiler compiler = new DefaultQueryCompiler();

            ISparqlQuery query = new SparqlQuery();
            query.AddProjectExpression("x", new CountAllAggregate());
            query.AddProjectExpression("y", new CountAllAggregate());

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            Assert.IsType(typeof(Project), algebra);
            Project project = (Project)algebra;
            Assert.Equal(2, project.Projections.Count);
            Assert.True(project.Projections.Contains("x"));

            Assert.IsType(typeof(Extend), project.InnerAlgebra);
            Extend extend = (Extend)project.InnerAlgebra;
            Assert.Equal(2, extend.Assignments.Count);
            Assert.Equal(query.Projections.First().Key, extend.Assignments.First().Key);
            Assert.Equal(query.Projections.Last().Key, extend.Assignments.Last().Key);

            Assert.IsType(typeof(GroupBy), extend.InnerAlgebra);
            GroupBy group = (GroupBy)extend.InnerAlgebra;
            Assert.Equal(0, group.GroupExpressions.Count);
            Assert.Equal(1, group.Aggregators.Count);
            Assert.Equal(query.Projections.First().Value, group.Aggregators.First().Key);
        }

        [Fact]
        public void QueryCompilerGroupBy3()
        {
            IQueryCompiler compiler = new DefaultQueryCompiler();

            ISparqlQuery query = new SparqlQuery();
            query.AddProjectExpression("x", new CountAllAggregate());
            query.AddProjectExpression("y", new CountAggregate(new VariableTerm("foo")));

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            Assert.IsType(typeof(Project), algebra);
            Project project = (Project)algebra;
            Assert.Equal(2, project.Projections.Count);
            Assert.True(project.Projections.Contains("x"));

            Assert.IsType(typeof(Extend), project.InnerAlgebra);
            Extend extend = (Extend)project.InnerAlgebra;
            Assert.Equal(2, extend.Assignments.Count);
            Assert.Equal(query.Projections.First().Key, extend.Assignments.First().Key);
            Assert.Equal(query.Projections.Last().Key, extend.Assignments.Last().Key);

            Assert.IsType(typeof(GroupBy), extend.InnerAlgebra);
            GroupBy group = (GroupBy)extend.InnerAlgebra;
            Assert.Equal(0, group.GroupExpressions.Count);
            Assert.Equal(2, group.Aggregators.Count);
            Assert.Equal(query.Projections.First().Value, group.Aggregators.First().Key);
            Assert.Equal(query.Projections.Last().Value, group.Aggregators.Last().Key);
        }

        [Fact]
        public void QueryCompilerHaving1()
        {
            IQueryCompiler compiler = new DefaultQueryCompiler();

            ISparqlQuery query = new SparqlQuery();
            query.AddProjectExpression("x", new CountAllAggregate());
            query.HavingConditions.Add(new GreaterThanExpression(new CountAllAggregate(), new ConstantTerm(new LongNode(100))));

            IAlgebra algebra = compiler.Compile(query);
            Console.WriteLine(algebra.ToString());

            Assert.IsType(typeof(Project), algebra);
            Project project = (Project) algebra;
            Assert.Equal(1, project.Projections.Count);
            Assert.True(project.Projections.Contains("x"));

            Assert.IsType(typeof(Extend), project.InnerAlgebra);
            Extend extend = (Extend)project.InnerAlgebra;
            Assert.Equal(1, extend.Assignments.Count);
            Assert.Equal(query.Projections.First().Key, extend.Assignments.First().Key);

            Assert.IsType(typeof(Filter), extend.InnerAlgebra);
            Filter having = (Filter)extend.InnerAlgebra;
            // Should not be equal because the 
            Assert.NotEqual(having.Expressions.First(), query.HavingConditions[0]);

            Assert.IsType(typeof(GroupBy), having.InnerAlgebra);
            GroupBy group = (GroupBy)having.InnerAlgebra;
            Assert.Equal(0, group.GroupExpressions.Count);
            Assert.Equal(1, group.Aggregators.Count);
            Assert.Equal(query.Projections.First().Value, group.Aggregators.First().Key);
        }
    }

    /// <summary>
    /// Tests for the <see cref="DefaultQueryCompiler"/>
    /// </summary>
    public class DefaultQueryCompilerTests
        : AbstractQueryCompilerTests
    {
        protected override IQueryCompiler CreateInstance()
        {
            return new DefaultQueryCompiler();
        }
    }
}