using System.Net;
using EventStore.ClientAPI;
using ReSTore.Infrastructure;
using StructureMap.Configuration.DSL;

namespace ReSTore.Views.Builders
{
    public class EventStoreRegistry : Registry
    {
        public EventStoreRegistry()
        {
            For<EventStoreConnection>().Singleton().Use(c =>
                {
                    var conn = EventStoreConnection.Create();
                    conn.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113));
                    return conn;
                });
            For<IEventStoreSerializer>().Use<JsonEventStoreSerializer>();
        }
    }
}