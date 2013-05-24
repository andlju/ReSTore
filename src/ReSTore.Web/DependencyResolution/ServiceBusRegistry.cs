using System;
using System.Configuration;
using MassTransit;
using ReSTore.Web.Controllers.OrderControllers;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace ReSTore.Web.DependencyResolution
{
    public class ServiceBusRegistry : Registry
    {
        public ServiceBusRegistry()
        {
            For<IServiceBus>().Singleton().Use(CreateMessageBus);
        }

        public static IServiceBus CreateMessageBus(IContext context)
        {
            var bus = ServiceBusFactory.New(cfg =>
            {
                cfg.ReceiveFrom("rabbitmq://localhost/restore-web");
                cfg.UseRabbitMq();
                cfg.Subscribe(subs =>
                    {
                        subs.Consumer<ViewModelNotifier>();
                    });
            });
            return bus;
        }
    }
}