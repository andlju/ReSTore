using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ReSTore.Infrastructure
{
    public class InMemoryRepository : IRepository<Guid>
    {
        private readonly Dictionary<Guid, List<object>> _store = new Dictionary<Guid, List<object>>();

        public T GetAggregate<T>(Guid id) where T : Aggregate, new()
        {
            List<object> storedEvents;
            if (!_store.TryGetValue(id, out storedEvents))
            {
                return null;
            }

            return AggregateHelper.Build<T>(storedEvents);
        }

        public void Store(Guid id, Aggregate aggregate, Action<IDictionary<string, object>> applyHeaders)
        {
            Store(id, aggregate.GetUncommittedEvents(), applyHeaders);
        }

        public void Store(Guid id, IEnumerable events, Action<IDictionary<string, object>> applyHeaders)
        {
            List<object> storedEvents;
            if (!_store.TryGetValue(id, out storedEvents))
            {
                storedEvents = new List<object>();
                _store[id] = storedEvents;
            }
            storedEvents.AddRange(events.OfType<object>());
        }

        public IEnumerable<object> GetEvents(Guid id)
        {
            List<object> storedEvents;
            if (!_store.TryGetValue(id, out storedEvents))
            {
                return null;
            }
            return storedEvents;
        }
    }
}