using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EventStore.ClientAPI;

namespace ReSTore.Infrastructure
{
    public class EventStoreRepository<TId> : IRepository<TId>
    {
        private readonly IEventStoreSerializer _serializer = new JsonEventStoreSerializer();

        public T GetAggregate<T>(TId id) where T : Aggregate, new()
        {
            var events = GetEvents(id);
            if (events == null)
                return null;
            return AggregateHelper.Build<T>(events);
        }

        public void Store(TId id, Aggregate aggregate, Action<IDictionary<string,object>> applyHeaders)
        {
            Store(id, aggregate.GetUncommittedEvents(), applyHeaders);
        }

        public void Store(TId id, IEnumerable events, Action<IDictionary<string, object>> applyHeaders)
        {
            using (var conn = EventStoreConnection.Create())
            {
                conn.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113));

                conn.AppendToStream(id.ToString(), ExpectedVersion.Any,
                                    events.OfType<object>()
                                          .Select(e => _serializer.Serialize(e, h =>
                                              {
                                                  h.Add("_Timestamp", DateTime.Now);
                                                  applyHeaders(h);
                                              })));
            }
        }

        public IEnumerable<object> GetEvents(TId id)
        {
            using (var conn = EventStoreConnection.Create())
            {
                conn.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113));

                var slice = conn.ReadStreamEventsForward(id.ToString(), 1, int.MaxValue, true);
                var events = slice.Events.
                                   Where(e => e.Event.EventType[0] != '$').
                                   Select(e => _serializer.Deserialize(e.Event)).
                                   Select(ec => ec.Event).ToArray();
                if (events.Length == 0)
                    return null;

                return events;
            }
        }
    }
}