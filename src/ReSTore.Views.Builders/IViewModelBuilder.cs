using System.Collections.Generic;
using ReSTore.Infrastructure;

namespace ReSTore.Views.Builders
{
    public interface IViewModelBuilder
    {
        string GetName();
        void Build<TId>(TId id, long fromEvent, long toEvent, IEnumerable<EventContext> events);
    }
}