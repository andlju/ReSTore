using System.Collections.Generic;
using ReSTore.Infrastructure;

namespace ReSTore.Views.Builders
{
    public interface IModelHandler<TModel>
    {
        void HandleAll(ref TModel model, IEnumerable<EventContext> events);
    }
}