
namespace VDS.RDF.Parsing
{
    public class GZippedTriXParser
        : BaseGZipParser
    {
        public GZippedTriXParser()
            : base(new TriXParser()) { }
    }
}
