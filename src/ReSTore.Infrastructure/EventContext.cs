using System.Collections.Generic;

namespace ReSTore.Infrastructure
{
    public class EventContext
    {
        public object Event { get; set; }
        public int EventNumber { get; set; }
        public IDictionary<string, object> Headers { get; set; }
    }
}