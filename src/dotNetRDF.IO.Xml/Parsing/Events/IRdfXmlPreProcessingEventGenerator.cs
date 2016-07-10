using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF.Parsing.Contexts;

namespace VDS.RDF.Parsing.Events
{
    /// <summary>
    /// Interface for event generators which generate all RDF/XML events in one go prior to parsing taking place
    /// </summary>
    public interface IRdfXmlPreProcessingEventGenerator
        : IPreProcessingEventGenerator<IRdfXmlEvent, RdfXmlParserContext>
    {
    }

    /// <summary>
    /// Interface for RDF/XML event generators which generate events as required during the parsing process
    /// </summary>
    public interface IRdfXmlJitEventGenerator
        : IJitEventGenerator<IRdfXmlEvent>
    {
    }

}
