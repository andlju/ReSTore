using System;
using MassTransit;


namespace ReSTore.Web.Controllers
{
    public interface ICommandDispatcher
    {
        void Dispatch<TMessage>(TMessage command) where TMessage : class;
    }

    public class MassTransitCommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceBus _bus;

        public MassTransitCommandDispatcher(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Dispatch<TMessage>(TMessage command) where TMessage : class
        {
            _bus.Publish(command);
        }
    }
}