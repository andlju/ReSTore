using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MassTransit;
using Raven.Client;
using ReSTore.Infrastructure;
using ReSTore.Messages.Notifications;

namespace ReSTore.Views.Builders
{
    public interface IModelUpdateNotifier
    {
        void Notify<TModel>(string id, TModel model);
    }

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

    public class RavenViewModelBuilder<TModel> : IViewModelBuilder
        where TModel : class
    {
        private readonly IDocumentStore _store;
        private readonly IModelHandler<TModel> _handler;
        private readonly IModelUpdateNotifier _updateNotifier;

        public RavenViewModelBuilder(IDocumentStore store, IModelHandler<TModel> handler, IModelUpdateNotifier updateNotifier)
        {
            _store = store;
            _handler = handler;
            _updateNotifier = updateNotifier;
        }

        public void Build<TId>(TId id, IEnumerable<EventContext> events)
        {
            var ravenId = typeof(TModel).Name + "/" + id;
            
            using (var session = _store.OpenSession())
            {
                var model = session.Load<TModel>(ravenId);
                
                var isNew = model == null;
                
                var eventNumber = _handler.HandleAll(ref model, events);

                if (model == null)
                    return;

                if (isNew)
                {
                    session.Store(model, ravenId);
                }
                
                var metaData = session.Advanced.GetMetadataFor(model);
                metaData["EventNumber"] = eventNumber;

                session.SaveChanges();
                _updateNotifier.Notify(id.ToString(), model);
            }
        }

    }
}