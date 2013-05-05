using System;
using EasyNetQ;

namespace ReSTore.Web.Controllers
{
    public interface ICommandDispatcher
    {
        void Dispatch<TMessage>(TMessage command);
    }

    public class EasyNetQCommandDispatcher : ICommandDispatcher
    {
        private IBus _bus;

        public EasyNetQCommandDispatcher(IBus bus)
        {
            _bus = bus;
        }

        public void Dispatch<TMessage>(TMessage command)
        {
            using (var channel = _bus.OpenPublishChannel())
            {
                channel.Publish(command);
            }
        }
    }
}