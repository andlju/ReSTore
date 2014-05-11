using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using ReSTore.Domain;
using ReSTore.Infrastructure;
using ReSTore.Views.Builders;
using StructureMap;

namespace ReSTore
{
    public class ViewBuilderEventDispatcher : IEventDispatcher
    {
        private readonly IContainer _container;

        public ViewBuilderEventDispatcher(IContainer container)
        {
            _container = container;
        }

        public void Dispatch(IEnumerable<EventContext> events)
        {
            var viewModelBuilders = _container.GetAllInstances<IViewModelBuilder>();

            foreach (var viewModelBuilder in viewModelBuilders)
            {

            }
        }
    }

    public class DomainService : IService
    {
        private readonly IContainer _container;
        private IServiceBus _bus;

        public DomainService(IContainer container)
        {
            _container = container;
        }

        public void Start()
        {
            _bus = _container.GetInstance<IServiceBus>();
            var handlerInterfaces = GetHandlerInterfaces();

            foreach (var handlerInterface in handlerInterfaces)
            {
                var msgType = handlerInterface.GetGenericArguments()[0];
                var handlerInstance =
                    _container.ForGenericType(typeof (ICommandHandler<>))
                              .WithParameters(msgType)
                              .GetInstanceAs<object>();

                var action = GetHandleAction(handlerInstance, msgType);

                Subscribe(_bus, msgType, action);
            }
        }

        private static Delegate GetHandleAction(object handlerInstance, Type msgType)
        {
            var handlerMethod = handlerInstance.GetType().GetMethod("Handle");
            var action = Delegate.CreateDelegate(typeof (Action<>).MakeGenericType(msgType),
                                                 handlerInstance, handlerMethod);
            return action;
        }

        private IEnumerable<Type> GetHandlerInterfaces()
        {
            return _container.Model.PluginTypes.
                              Where(
                                  p =>
                                  p.PluginType.IsGenericType &&
                                  p.PluginType.GetGenericTypeDefinition() == typeof (ICommandHandler<>)).
                              Select(p => p.PluginType).ToArray();
        }

        private static void Subscribe(IServiceBus bus, Type msgType, object action)
        {
            var subMethod = typeof(HandlerSubscriptionExtensions).GetMethods().Single(m => m.Name == "SubscribeHandler" && m.GetParameters().Length == 2).MakeGenericMethod(msgType);
            
            subMethod.Invoke(bus, new object[] { bus, action });
        }

        public void Stop()
        {
            _bus.Dispose();
        }
    }
}