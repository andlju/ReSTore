using EventStore.ClientAPI;

namespace ReSTore.Infrastructure
{
    public interface IEventStoreSerializer
    {
        EventData Serialize(object testClassToSerialize);
        object Deserialize(RecordedEvent evt);
    }
}