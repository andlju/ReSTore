using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

namespace ReSTore.Infrastructure
{
    public class InMemoryRepository : IRepository<Guid>
    {
        private readonly Dictionary<Guid, List<EventContext>> _store = new Dictionary<Guid, List<EventContext>>();
        private readonly IList<IEventDispatcher> _eventDispatchers = new List<IEventDispatcher>();

        public T GetAggregate<T>(Guid id) where T : Aggregate, new()
        {
            List<EventContext> storedEvents;
            if (!_store.TryGetValue(id, out storedEvents))
            {
                return null;
            }

            return AggregateHelper.Build<T>(storedEvents.Select(ec => ec.Event));
        }

        public void Store(Guid id, Aggregate aggregate, Action<IDictionary<string, object>> applyHeaders)
        {
            Store(id, aggregate.GetUncommittedEvents(), applyHeaders);
        }

        public void Store(Guid id, IEnumerable events, Action<IDictionary<string, object>> applyHeaders)
        {
            List<EventContext> storedEvents;
            if (!_store.TryGetValue(id, out storedEvents))
            {
                storedEvents = new List<EventContext>();
                _store[id] = storedEvents;
            }
            var lastIndex = storedEvents.Count;
            var eventContexts = events.OfType<object>().Select(e =>
            {
                var ec = new EventContext()
                {
                    Event = e,
                    EventNumber = lastIndex++,
                    Headers = new Dictionary<string, object>()
                };
                applyHeaders(ec.Headers);
                return ec;
            }).ToArray();

            storedEvents.AddRange(eventContexts);
            foreach (var dispatcher in _eventDispatchers)
            {
                dispatcher.Dispatch(eventContexts);
            }
        }

        public IEnumerable<object> GetEvents(Guid id)
        {
            List<EventContext> storedEvents;
            if (!_store.TryGetValue(id, out storedEvents))
            {
                return null;
            }
            return storedEvents.Select(ec => ec.Event);
        }

        public void RegisterDispatcher(IEventDispatcher eventDispatcher)
        {
            _eventDispatchers.Add(eventDispatcher);
        }
    }
}