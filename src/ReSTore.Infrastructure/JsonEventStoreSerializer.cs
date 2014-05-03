using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Utils;

namespace ReSTore.Infrastructure
{
    public class JsonEventStoreSerializer : IEventStoreSerializer
    {
        private const string ClrTypeHeader = "_CLRType";

        public EventData Serialize(object testClassToSerialize, Action<Dictionary<string, object>> setHeaders)
        {
            var headers = new Dictionary<string, object>();
            if (setHeaders != null)
                setHeaders(headers);

            var type = testClassToSerialize.GetType();

            headers.Add(ClrTypeHeader, type.AssemblyQualifiedName);

            var eventData = GetJsonBytes(testClassToSerialize);
            var headersData = GetJsonBytes(headers);

            return new EventData(Guid.NewGuid(), type.Name, true, eventData, headersData);
        }

        public EventContext Deserialize(RecordedEvent evt)
        {
            var headers = GetObject<Dictionary<string, object>>(evt.Metadata);

            var type = Type.GetType((string)headers[ClrTypeHeader]);
            
            var eventData = evt.Data;
            var obj = GetObject(eventData, type);
            
            return new EventContext() { Event = obj, EventNumber = evt.EventNumber, Headers = headers };
        }

        private static byte[] GetJsonBytes(object testClassToSerialize)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(testClassToSerialize);
            var data = Encoding.UTF8.GetBytes(json);
            return data;
        }

        private static object GetObject(byte[] eventData, Type type)
        {
            var json = Encoding.UTF8.GetString(eventData);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);
            return obj;
        }

        private static T GetObject<T>(byte[] eventData)
        {
            var json = Encoding.UTF8.GetString(eventData);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            return obj;
        }
    }
}