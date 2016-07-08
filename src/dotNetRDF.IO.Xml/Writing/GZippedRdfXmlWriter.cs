using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDS.RDF.Writing
{
    /// <summary>
    /// Writer for GZipped RDF/XML
    /// </summary>
    public class GZippedRdfXmlWriter
        : BaseGZipWriter
    {
        /// <summary>
        /// Creates a new GZipped RDF/XML writer
        /// </summary>
        public GZippedRdfXmlWriter()
            : base(new RdfXmlWriter()) { }
    }


}
