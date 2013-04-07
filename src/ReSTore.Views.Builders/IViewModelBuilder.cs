using System.Collections.Generic;
using ReSTore.Infrastructure;

namespace ReSTore.Views.Builders
{
    public interface IViewModelBuilder
    {
        void Build<TId>(TId id, IEnumerable<EventContext> events);
    }
}