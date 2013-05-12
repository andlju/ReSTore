using System;
using System.Collections.Generic;
using System.Linq;
using EasyNetQ;
using ReSTore.Domain;
using StructureMap;

namespace ReSTore
{
    public class DomainService : IService
    {
        private readonly IContainer _container;
        private IBus _bus;

        public DomainService(IContainer container)
        {
            _container = container;
        }

        public void Start()
        {
            _bus = _container.GetInstance<IBus>();
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

        private static void Subscribe(IBus bus, Type msgType, object action)
        {
            var subMethod = bus.GetType().GetMethods().Single(m => m.Name == "Subscribe" && m.GetParameters().Length == 2).MakeGenericMethod(msgType);
            
            subMethod.Invoke(bus, new object[] { "test", action });
        }

        public void Stop()
        {
            _bus.Dispose();
        }
    }
}