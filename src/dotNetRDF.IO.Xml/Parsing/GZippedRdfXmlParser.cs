using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDS.RDF.Parsing
{
    /// <summary>
    /// Parser for loading GZipped RDF/XML
    /// </summary>
    public class GZippedRdfXmlParser
        : BaseGZipParser
    {
        /// <summary>
        /// Creates a new GZipped RDF/XML parser
        /// </summary>
        public GZippedRdfXmlParser()
            : base(new RdfXmlParser()) { }

        /// <summary>
        /// Creates a new GZipped RDF/XML parser
        /// </summary>
        /// <param name="mode">RDF/XML parser mode</param>
        public GZippedRdfXmlParser(RdfXmlParserMode mode)
            : base(new RdfXmlParser(mode)) { }
    }
}
