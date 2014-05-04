using System;
using System.Diagnostics;
using MassTransit;
using ReSTore.Messages.Notifications;

namespace ReSTore.Views.Builders
{
    public class ServiceBusUpdateNotifier : IModelUpdateNotifier
    {
        private IServiceBus _bus;

        public ServiceBusUpdateNotifier(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Notify<TModel>(string id, TModel model)
        {
            _bus.Publish(new ViewModelUpdated()
            {
                Id = Guid.Parse(id),
                Type = typeof (TModel).Name,
                Content = model
            });
            Debug.WriteLine(string.Format("ViewModelUpdated published: {0} {1}", id, typeof(TModel).Name));
        }
    }
}