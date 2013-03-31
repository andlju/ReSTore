using System;
using EasyNetQ;

namespace ReSTore.Web.Controllers
{
    public interface ICommandDispatcher
    {
        void Dispatch<TMessage>(Guid commandId, TMessage command);
    }

    public class EasyNetQCommandDispatcher : ICommandDispatcher
    {
        private IBus _bus;

        public EasyNetQCommandDispatcher(IBus bus)
        {
            _bus = bus;
        }

        public void Dispatch<TMessage>(Guid commandId, TMessage command)
        {
            using (var channel = _bus.OpenPublishChannel())
            {
                channel.Publish(command);
            }
        }
    }
}