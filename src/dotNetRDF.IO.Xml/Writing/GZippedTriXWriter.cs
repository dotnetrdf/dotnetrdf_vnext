using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDS.RDF.Writing
{
    public class GZippedTriXWriter
        : BaseGZipWriter
    {
        public GZippedTriXWriter()
            : base(new TriXWriter()) { }
    }

}
