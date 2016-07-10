using Xunit;
using VDS.RDF.Graphs;

namespace VDS.RDF.Query.Processors
{
    public class GraphQueryProcessorTests
        : AbstractQueryProcessorTests
    {
        protected override IQueryProcessor CreateProcessor(IGraph g)
        {
            return new GraphQueryProcesor(g);
        }
    }
}
