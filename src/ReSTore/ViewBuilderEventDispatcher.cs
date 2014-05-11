using System;
using System.Collections.Generic;
using System.Linq;
using ReSTore.Infrastructure;
using ReSTore.Views.Builders;
using StructureMap;

namespace ReSTore
{
    public class ViewBuilderEventDispatcher : IEventDispatcher<Guid>
    {
        private readonly IContainer _container;

        public ViewBuilderEventDispatcher(IContainer container)
        {
            _container = container;
        }

        public void Dispatch(Guid id, IEnumerable<EventContext> events)
        {
            var viewModelBuilders = _container.GetAllInstances<IViewModelBuilder>();
            var contexts = events.ToArray();
            foreach (var viewModelBuilder in viewModelBuilders)
            {
                viewModelBuilder.Build(id, contexts.First().EventNumber, contexts.Last().EventNumber, contexts);
            }
        }
    }
}