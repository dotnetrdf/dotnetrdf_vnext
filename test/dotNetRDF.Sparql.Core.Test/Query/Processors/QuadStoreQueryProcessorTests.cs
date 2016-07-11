using Xunit;
using VDS.RDF.Graphs;

namespace VDS.RDF.Query.Processors
{
    public class QuadStoreQueryProcessorTests
        : AbstractQueryProcessorTests
    {
        protected override IQueryProcessor CreateProcessor(IGraph g)
        {
            GraphStore gs = new GraphStore();
            gs.Add(g);
            return new QuadStoreQueryProcessor(gs);
        }
    }
}
