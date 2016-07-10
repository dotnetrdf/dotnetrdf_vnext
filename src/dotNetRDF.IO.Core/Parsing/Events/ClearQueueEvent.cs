using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VDS.RDF.Parsing.Events
{
    /// <summary>
    /// An Event for representing that the Event Queue should be cleared of previously queued events
    /// </summary>
    public class ClearQueueEvent : BaseEvent
    {
        /// <summary>
        /// Creates a new Clear Queue Event
        /// </summary>
        public ClearQueueEvent()
            : base(BaseEvent.Clear) { }
    }
}
