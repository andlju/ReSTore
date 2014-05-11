using System;
using System.Collections.Generic;
using EventStore.ClientAPI;

namespace ReSTore.Infrastructure
{
    public interface IEventStoreSerializer
    {
        EventData Serialize(object obj, Action<Dictionary<string, object>> setHeaders);
        EventContext Deserialize(RecordedEvent evt);
    }
}