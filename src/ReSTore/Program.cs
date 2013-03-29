using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using ReSTore.Domain;
using ReSTore.Domain.CommandHandlers;
using ReSTore.Infrastructure;
using ReSTore.Messages.Events;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using Topshelf;

namespace ReSTore
{
    public class DomainRegistry : Registry
    {
        public DomainRegistry()
        {
            Scan(s =>
                {
                    s.AssemblyContainingType<CreateOrderHandler>();
                    s.ConnectImplementationsToTypesClosing(typeof (ICommandHandler<>));
                });
            For<IRepository<Guid>>().Use<EventStoreRepository<Guid>>();
        }
    }


    public class ServiceBusRegistry : Registry
    {
        public ServiceBusRegistry()
        {
            For<IBus>().Use(CreateMessageBus);
        }

        public static IBus CreateMessageBus(IContext context)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EasyNetQ"];
            if (connectionString == null || connectionString.ConnectionString == string.Empty)
            {
                throw new Exception("EasyNetQ connection string is missing or empty");
            }

            var bus = RabbitHutch.CreateBus(connectionString.ConnectionString);

            return bus;
        }
    }

    public interface IService
    {
        void Start();
        void Stop();
    }

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

    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container(x =>
                {
                    x.AddRegistry<DomainRegistry>();
                    x.AddRegistry<ServiceBusRegistry>();

                    x.For<IService>().Use<DomainService>();
                });

            HostFactory.Run(x =>
                {

                    x.Service<IService>(c =>
                        {
                            c.ConstructUsing(() => container.GetInstance<IService>());
                            c.WhenStarted(s => s.Start());
                            c.WhenStopped(s => s.Stop());
                        });

                    x.RunAsLocalSystem();
                    x.SetServiceName("ReSTore.Orders");
                    x.SetDisplayName("ReSTore Orders service");
                    x.SetDescription("Main service for the ReSTore order domain");
                });

        }
    }
}
