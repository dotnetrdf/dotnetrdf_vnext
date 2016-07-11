using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Xunit;
using VDS.RDF.Graphs;
using VDS.RDF.Nodes;
using VDS.RDF.Query.Elements;
using VDS.RDF.Query.Expressions;
using VDS.RDF.Query.Expressions.Aggregates;
using VDS.RDF.Query.Expressions.Aggregates.Sparql;
using VDS.RDF.Query.Expressions.Primary;
using VDS.RDF.Query.Results;
using VDS.RDF.Query.Sorting;

namespace VDS.RDF.Query.Processors
{
    public abstract class AbstractQueryProcessorTests
    {
        private static IGraph CreateGraph()
        {
            IGraph g = new Graph();
            g.Namespaces.AddNamespace(String.Empty, new Uri("http://test/"));

            INode s1 = g.CreateUriNode(":s");
            INode s2 = g.CreateBlankNode();
            INode p1 = g.CreateUriNode(":p1");
            INode p2 = g.CreateUriNode(":p2");
            INode o1 = g.CreateUriNode(":o");
            INode o2 = g.CreateLiteralNode("object");

            g.Assert(s1, p1, o1);
            g.Assert(s1, p1, o2);
            g.Assert(s1, p2, o1);
            g.Assert(s1, p2, o1);
            g.Assert(s1, p1, s2);

            g.Assert(s2, p1, o1);

            return g;
        }

        /// <summary>
        /// Creates a processor that can operate over the given graph
        /// </summary>
        /// <param name="g">Graph</param>
        /// <returns></returns>
        protected abstract IQueryProcessor CreateProcessor(IGraph g);

        [Fact]
        public void QueryProcessorAskEmptyWhere()
        {
            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.Ask;

            IQueryProcessor processor = CreateProcessor(CreateGraph());
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsBoolean);
            Assert.True(result.Boolean.HasValue);
            Assert.True(result.Boolean.Value);
        }

        [Fact]
        public void QueryProcessorAskWhereNoMatches()
        {
            IGraph g = CreateGraph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.Ask;
            INode noSuchThing = g.CreateUriNode(":nosuchthing");
            Triple t = new Triple(noSuchThing, noSuchThing, noSuchThing);
            query.WhereClause = new TripleBlockElement(t.AsEnumerable());

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsBoolean);
            Assert.True(result.Boolean.HasValue);
            Assert.False(result.Boolean.Value);
        }

        [Fact]
        public void QueryProcessorAskWhereAnyMatches()
        {
            IGraph g = CreateGraph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.Ask;
            Triple t = new Triple(g.CreateVariableNode("s"), g.CreateVariableNode("p"), g.CreateVariableNode("o"));
            query.WhereClause = new TripleBlockElement(t.AsEnumerable());

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsBoolean);
            Assert.True(result.Boolean.HasValue);
            Assert.True(result.Boolean.Value);
        }

        [Fact]
        public void QueryProcessorAskWhereConcreteMatch()
        {
            IGraph g = CreateGraph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.Ask;
            Triple t = g.Triples.FirstOrDefault(x => x.IsGround);
            Assert.NotNull(t);
            query.WhereClause = new TripleBlockElement(t.AsEnumerable());

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsBoolean);
            Assert.True(result.Boolean.HasValue);
            Assert.True(result.Boolean.Value);
        }

        [Fact]
        public void QueryProcessorSelectEmptyWhere()
        {
            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.SelectAll;

            IQueryProcessor processor = CreateProcessor(CreateGraph());
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsTabular);
            IRandomAccessTabularResults results = new RandomAccessTabularResults(result.Table);
            Assert.Equal(1, results.Count);

            IResultRow row = results[0];
            Assert.True(row.IsEmpty);
        }

        [Fact]
        public void QueryProcessorSelectWhereNoMatches()
        {
            IGraph g = CreateGraph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.SelectAll;
            INode noSuchThing = g.CreateUriNode(":nosuchthing");
            Triple t = new Triple(noSuchThing, noSuchThing, noSuchThing);
            query.WhereClause = new TripleBlockElement(t.AsEnumerable());

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsTabular);
            IRandomAccessTabularResults results = new RandomAccessTabularResults(result.Table);
            Assert.Equal(0, results.Count);
        }

        [Fact]
        public void QueryProcessorSelectWhereAnyMatches()
        {
            IGraph g = CreateGraph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.SelectAll;
            Triple t = new Triple(g.CreateVariableNode("s"), g.CreateVariableNode("p"), g.CreateVariableNode("o"));
            query.WhereClause = new TripleBlockElement(t.AsEnumerable());

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsTabular);
            IRandomAccessTabularResults results = new RandomAccessTabularResults(result.Table);
            Assert.Equal(g.Count, results.Count);

            Assert.True(results.All(r => r.HasBoundValue("s") && r.HasBoundValue("p") && r.HasBoundValue("o")));
        }

        [Fact]
        public void QueryProcessorSelectWhereConcreteMatch()
        {
            IGraph g = CreateGraph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.SelectAll;
            Triple t = g.Triples.FirstOrDefault(x => x.IsGround);
            Assert.NotNull(t);
            query.WhereClause = new TripleBlockElement(t.AsEnumerable());

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsTabular);
            IRandomAccessTabularResults results = new RandomAccessTabularResults(result.Table);
            Assert.Equal(1, results.Count);

            IResultRow row = results[0];
            Assert.True(row.IsEmpty);
        }

        [Fact]
        public void QueryProcessorProject1()
        {
            IGraph g = new Graph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.Select;
            query.AddProjectVariable("x");

            // Form a VALUES clause for the WHERE
            INode ten = new LongNode(10);
            INode hundred = new LongNode(100);
            IMutableResultRow r1 = new MutableResultRow(new String[] { "x", "y"});
            r1.Set("x", ten);
            r1.Set("y", hundred);
            IMutableTabularResults data = new MutableTabularResults(r1.Variables, new IMutableResultRow[] { r1 });
            query.WhereClause = new DataElement(data);

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsTabular);
            IRandomAccessTabularResults results = new RandomAccessTabularResults(result.Table);
            Assert.Equal(1, results.Count);

            IResultRow row = results[0];
            Assert.True(row.HasBoundValue("x"));
            Assert.Equal(ten, row["x"]);
        }

        [Fact]
        public void QueryProcessorProject2()
        {
            IGraph g = new Graph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.Select;
            query.AddProjectVariable("x");

            // Form a VALUES clause for the WHERE
            INode ten = new LongNode(10);
            INode hundred = new LongNode(100);
            IMutableResultRow r1 = new MutableResultRow(new String[] { "x", "y" });
            r1.Set("x", ten);
            r1.Set("y", hundred);
            IMutableResultRow r2 = new MutableResultRow(new String[] { "x" });
            r2.Set("x", ten);
            IMutableResultRow r3 = new MutableResultRow(new String[] { "y"});
            r3.Set("y", hundred);
            IMutableResultRow r4 = new MutableResultRow();
            IMutableTabularResults data = new MutableTabularResults(r1.Variables, new IMutableResultRow[] { r1, r2, r3, r4 });
            query.WhereClause = new DataElement(data);

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsTabular);
            IRandomAccessTabularResults results = new RandomAccessTabularResults(result.Table);
            Assert.Equal(data.Count, results.Count);

            foreach (IResultRow row in results)
            {
                Assert.True(row.HasValue("x"));
                if (row.HasBoundValue("x"))
                {
                    Assert.Equal(ten, row["x"]);
                }
                else
                {
                    Assert.False(row.HasBoundValue("x"));
                }
            }
        }

        [Fact]
        public void QueryProcessorOrderBy1()
        {
            IGraph g = new Graph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.Select;
            query.SortConditions = new ISortCondition[] { new SortCondition(new VariableTerm("x")) };

            // Form a VALUES clause for the WHERE
            INode ten = new LongNode(10);
            INode hundred = new LongNode(100);
            IMutableResultRow r1 = new MutableResultRow(new String[] { "x" });
            r1.Set("x", ten);
            IMutableResultRow r2 = new MutableResultRow(new String[] { "x" });
            r2.Set("x", hundred);
            IMutableTabularResults data = new MutableTabularResults(r1.Variables, new IMutableResultRow[] { r1, r2 });
            query.WhereClause = new DataElement(data);

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsTabular);
            IRandomAccessTabularResults results = new RandomAccessTabularResults(result.Table);
            Assert.Equal(data.Count, results.Count);

            IResultRow first = results[0];
            IResultRow second = results[1];
            Assert.True(first.HasBoundValue("x"));
            Assert.True(second.HasBoundValue("x"));
            Assert.Equal(ten, first["x"]);
            Assert.Equal(hundred, second["x"]);
        }

        [Fact]
        public void QueryProcessorOrderBy2()
        {
            IGraph g = new Graph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.Select;
            query.SortConditions = new ISortCondition[] { new SortCondition(new VariableTerm("x"), false) };

            // Form a VALUES clause for the WHERE
            INode ten = new LongNode(10);
            INode hundred = new LongNode(100);
            IMutableResultRow r1 = new MutableResultRow(new String[] { "x" });
            r1.Set("x", ten);
            IMutableResultRow r2 = new MutableResultRow(new String[] { "x" });
            r2.Set("x", hundred);
            IMutableTabularResults data = new MutableTabularResults(r1.Variables, new IMutableResultRow[] { r1, r2 });
            query.WhereClause = new DataElement(data);

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsTabular);
            IRandomAccessTabularResults results = new RandomAccessTabularResults(result.Table);
            Assert.Equal(data.Count, results.Count);

            IResultRow first = results[0];
            IResultRow second = results[1];
            Assert.True(first.HasBoundValue("x"));
            Assert.True(second.HasBoundValue("x"));
            Assert.Equal(hundred, first["x"]);
            Assert.Equal(ten, second["x"]);
        }

        [Fact]
        public void QueryProcessorGroupBy1()
        {
            IGraph g = new Graph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.Select;
            query.AddProjectVariable("x");
            query.AddProjectExpression("count", new CountAllAggregate());
            query.GroupExpressions.Add(new KeyValuePair<IExpression, string>(new VariableTerm("x"), "x"));

            // Form a VALUES clause for the WHERE
            INode ten = new LongNode(10);
            INode hundred = new LongNode(100);
            IMutableResultRow r1 = new MutableResultRow(new String[] { "x", "y" });
            r1.Set("x", ten);
            r1.Set("y", hundred);
            IMutableResultRow r2 = new MutableResultRow(new String[] { "x" });
            r2.Set("x", ten);
            IMutableResultRow r3 = new MutableResultRow(new String[] { "y" });
            r3.Set("y", hundred);
            IMutableResultRow r4 = new MutableResultRow();
            IMutableTabularResults data = new MutableTabularResults(r1.Variables, new IMutableResultRow[] { r1, r2, r3, r4 });
            query.WhereClause = new DataElement(data);

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsTabular);
            IRandomAccessTabularResults results = new RandomAccessTabularResults(result.Table);
            QueryTestTools.ShowResults(results);
            Assert.Equal(2, results.Count);

            foreach (IResultRow row in results)
            {
                Assert.Equal(2, row.Variables.Count());
                Assert.True(row.HasValue("x"));
                if (row.HasBoundValue("x"))
                {
                    Assert.Equal(ten, row["x"]);
                }
                Assert.True(row.HasBoundValue("count"));
                Assert.Equal(new LongNode(2), row["count"]);
            }
        }

        [Fact]
        public void QueryProcessorGroupBy2()
        {
            IGraph g = new Graph();

            ISparqlQuery query = new SparqlQuery();
            query.QueryType = QueryType.Select;
            query.AddProjectVariable("x");
            query.AddProjectExpression("sample", new SampleAggregate(new VariableTerm("y")));
            query.GroupExpressions.Add(new KeyValuePair<IExpression, string>(new VariableTerm("x"), "x"));

            // Form a VALUES clause for the WHERE
            INode ten = new LongNode(10);
            INode hundred = new LongNode(100);
            IMutableResultRow r1 = new MutableResultRow(new String[] { "x", "y" });
            r1.Set("x", ten);
            r1.Set("y", hundred);
            IMutableResultRow r2 = new MutableResultRow(new String[] { "x" });
            r2.Set("x", ten);
            IMutableResultRow r3 = new MutableResultRow(new String[] { "y" });
            r3.Set("y", hundred);
            IMutableResultRow r4 = new MutableResultRow();
            IMutableTabularResults data = new MutableTabularResults(r1.Variables, new IMutableResultRow[] { r1, r2, r3, r4 });
            query.WhereClause = new DataElement(data);

            IQueryProcessor processor = CreateProcessor(g);
            IQueryResult result = processor.Execute(query);

            Assert.True(result.IsTabular);
            IRandomAccessTabularResults results = new RandomAccessTabularResults(result.Table);
            QueryTestTools.ShowResults(results);
            Assert.Equal(2, results.Count);

            foreach (IResultRow row in results)
            {
                Assert.Equal(2, row.Variables.Count());
                Assert.True(row.HasValue("x"));
                if (row.HasBoundValue("x"))
                {
                    Assert.Equal(ten, row["x"]);
                }
                Assert.True(row.HasValue("sample"));
                if (row.HasBoundValue("sample"))
                {
                    Assert.Equal(hundred, row["sample"]);
                }
            }
        }
    }
}
