using System;
using System.Text;
using EventStore.ClientAPI;

namespace ReSTore.Infrastructure
{
    public class JsonEventStoreSerializer : IEventStoreSerializer
    {
        public EventData Serialize(object testClassToSerialize)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(testClassToSerialize);
            var data = Encoding.UTF8.GetBytes(json);

            return new EventData(Guid.NewGuid(), testClassToSerialize.GetType().AssemblyQualifiedName, true, data, null);
        }

        public object Deserialize(RecordedEvent evt)
        {
            var type = Type.GetType(evt.EventType);
            var json = Encoding.UTF8.GetString(evt.Data);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
            return obj;
        }
    }
}