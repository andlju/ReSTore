using System.Collections.Generic;

namespace ReSTore.Infrastructure
{
    public interface IRepository<TId>
    {
        T GetAggregate<T>(TId id) where T : Aggregate, new();
        void Store(TId id, Aggregate aggregate);
        IEnumerable<object> GetEvents(TId id);
    }

}
