using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace ReSTore.Infrastructure
{
    public class EventStoreRepository : IRepository<Guid>
    {
        private readonly IEventStoreSerializer _serializer = new JsonEventStoreSerializer();
        private readonly IEventStoreConnection _connection;
        private readonly IList<IEventDispatcher<Guid>> _eventDispatchers = new List<IEventDispatcher<Guid>>();

        public EventStoreRepository()
        {
            ConnectionSettings settings = ConnectionSettings.Create();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113);
            _connection = EventStoreConnection.Create(settings, endpoint);
            _connection.Connect();
            _connection.SubscribeToAll(true, (s, e) =>
            {
                if (e.Event.EventType.StartsWith("$"))
                    return;
                foreach (var dispatcher in _eventDispatchers)
                {
                    var eventContext = _serializer.Deserialize(e.Event);
                    dispatcher.Dispatch(Guid.Parse(e.OriginalStreamId), new [] {eventContext});
                }
            },null, new UserCredentials("admin", "changeit"));
        }

        public T GetAggregate<T>(Guid id) where T : Aggregate, new()
        {
            var events = GetEvents(id);
            if (events == null)
                return null;
            return AggregateHelper.Build<T>(events);
        }

        public void Store(Guid id, Aggregate aggregate, Action<IDictionary<string,object>> applyHeaders)
        {
            Store(id, aggregate.GetUncommittedEvents(), applyHeaders);
        }

        public void Store(Guid id, IEnumerable events, Action<IDictionary<string, object>> applyHeaders)
        {
            var eventsArray = events as object[] ?? events.Cast<object>().ToArray();
            _connection.AppendToStream(id.ToString(), ExpectedVersion.Any,
                eventsArray
                    .Select(e => _serializer.Serialize(e, h =>
                    {
                        h.Add("_Timestamp", DateTime.Now);
                        applyHeaders(h);
                    })));
        }

        public IEnumerable<object> GetEvents(Guid id)
        {

            // TODO Support more than 100 events
            var slice = _connection.ReadStreamEventsForward(id.ToString(), StreamPosition.Start, 100, true);
            var events = slice.Events.
                                Where(e => e.Event.EventType[0] != '$').
                                Select(e => _serializer.Deserialize(e.Event)).
                                Select(ec => ec.Event).ToArray();
            if (events.Length == 0)
                return null;

            return events;
        }

        public void RegisterDispatcher(IEventDispatcher<Guid> eventDispatcher)
        {
            _eventDispatchers.Add(eventDispatcher);
        }
    }
}