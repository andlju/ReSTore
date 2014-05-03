using System.Net;
using EventStore.ClientAPI;
using ReSTore.Infrastructure;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace ReSTore.Views.Builders
{
    public class EventStoreRegistry : Registry
    {
        public EventStoreRegistry()
        {
            For<IEventStoreConnection>().Singleton().Use("Creating EventStore connection", c =>
            {
                var settings = ConnectionSettings.Create();
                var endpoint = new IPEndPoint(IPAddress.Loopback, 1113);
                var conn = EventStoreConnection.Create(settings, endpoint);
                conn.Connect();
                return conn;
            });
            For<IEventStoreSerializer>().Use<JsonEventStoreSerializer>();
        }
    }
}