using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDS.RDF.Parsing.Events
{
    /// <summary>
    /// Abstract Base Class for <see cref="IRdfXmlEvent">IRdfXmlEvent</see> implementations
    /// </summary>
    public abstract class BaseRdfXmlEvent
        : BaseEvent, IRdfXmlEvent
    {
        private String _sourcexml;

        /// <summary>
        /// Creates an Event and fills in its Values
        /// </summary>
        /// <param name="eventType">Type of the Event</param>
        /// <param name="sourceXml">Source XML that generated the Event</param>
        /// <param name="pos">Position of the XML Event</param>
        public BaseRdfXmlEvent(int eventType, String sourceXml, PositionInfo pos)
            : base(eventType, pos)
        {
            this._sourcexml = sourceXml;
        }

        /// <summary>
        /// Creates an Event and fills in its Values
        /// </summary>
        /// <param name="eventType">Type of the Event</param>
        /// <param name="sourceXml">Source XML that generated the Event</param>
        public BaseRdfXmlEvent(int eventType, String sourceXml)
            : this(eventType, sourceXml, null) { }

        /// <summary>
        /// Gets the XML that this Event was generated from
        /// </summary>
        public string SourceXml
        {
            get
            {
                return this._sourcexml;
            }
        }
    }
}
