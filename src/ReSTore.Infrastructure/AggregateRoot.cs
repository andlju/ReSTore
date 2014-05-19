using System.Collections.Generic;
using System.Linq;

namespace ReSTore.Infrastructure
{
    public abstract class AggregateRoot
    {
        private readonly IList<object> _uncommittedEvents = new List<object>();

        public void Raise(object evt)
        {
            this.Apply(evt);
            _uncommittedEvents.Add(evt);
        }

        public IEnumerable<object> FetchUncommittedEvents()
        {
            var events = _uncommittedEvents.ToArray();
            _uncommittedEvents.Clear();
            return events;
        }
    }
}