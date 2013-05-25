using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ReSTore.Infrastructure;

namespace ReSTore.Domain.Tests.Infrastructure
{
    public class MockRepository : IRepository<Guid>
    {
        private readonly Dictionary<Guid, List<object>> _store = new Dictionary<Guid, List<object>>();
        private readonly Dictionary<Guid, List<object>> _committedStore = new Dictionary<Guid, List<object>>();

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
            var events = aggregate.GetUncommittedEvents();
            Store(id, events, _store);
            Store(id, events, _committedStore);
        }

        public void Store(Guid id, IEnumerable<object> events, Action<IDictionary<string, object>> applyHeaders)
        {
            var evts = events.ToArray();
            Store(id, evts, _store);
            Store(id, evts, _committedStore);
        }

        private static void Store(Guid id, IEnumerable<object> events, Dictionary<Guid, List<object>> store)
        {
            List<object> storedEvents;
            if (!store.TryGetValue(id, out storedEvents))
            {
                storedEvents = new List<object>();
                store[id] = storedEvents;
            }
            storedEvents.AddRange(events);
        }
        
        public void ResetCommitted()
        {
            _committedStore.Clear();
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

        public IEnumerable<object> GetCommittedEvents(Guid id)
        {
            List<object> storedEvents;
            if (!_committedStore.TryGetValue(id, out storedEvents))
            {
                return Enumerable.Empty<object>();
            }
            return storedEvents;
        }
        
    }
}