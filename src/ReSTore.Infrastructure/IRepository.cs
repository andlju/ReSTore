using System;
using System.Collections.Generic;

namespace ReSTore.Infrastructure
{
    public interface IEventDispatcher<TId>
    {
        void Dispatch(TId id, IEnumerable<EventContext> events);
    }

    public interface IRepository<TId>
    {
        T GetAggregate<T>(TId id) where T : AggregateRoot, new();
        void Store(TId id, AggregateRoot aggregateRoot, Action<IDictionary<string,object>> applyHeaders);
        IEnumerable<object> GetEvents(TId id);
        void RegisterDispatcher(IEventDispatcher<TId> eventDispatcher);
    }

}
