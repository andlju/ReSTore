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
        private readonly IList<IEventDispatcher<Guid>> _eventDispatchers = new List<IEventDispatcher<Guid>>();

        public T GetAggregate<T>(Guid id) where T : AggregateRoot, new()
        {
            List<EventContext> storedEvents;
            if (!_store.TryGetValue(id, out storedEvents))
            {
                return null;
            }

            return AggregateHelper.Build<T>(storedEvents.Select(ec => ec.Event));
        }

        public void Store(Guid id, AggregateRoot aggregateRoot, Action<IDictionary<string, object>> applyHeaders)
        {
            Store(id, aggregateRoot.FetchUncommittedEvents(), applyHeaders);
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
                dispatcher.Dispatch(id, eventContexts);
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

        public void RegisterDispatcher(IEventDispatcher<Guid> eventDispatcher)
        {
            _eventDispatchers.Add(eventDispatcher);
        }
    }
}