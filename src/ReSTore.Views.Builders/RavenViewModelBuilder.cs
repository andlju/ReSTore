using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using ReSTore.Infrastructure;

namespace ReSTore.Views.Builders
{
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

        public string GetName()
        {
            return typeof (TModel).Name;
        }

        public void Build<TId>(TId id, long fromEvent, long toEvent, IEnumerable<EventContext> events)
        {
            var ravenId = typeof(TModel).Name + "/" + id;
            
            TModel model;
            using (var session = _store.OpenSession())
            {
                model = session.Load<TModel>(ravenId);

                var isNew = model == null;
                
                _handler.HandleAll(ref model, events);

                if (model == null)
                    return;

                if (isNew)
                {
                    session.Store(model, ravenId);
                }
                
                var metaData = session.Advanced.GetMetadataFor(model);
                metaData["EventNumber"] = toEvent;

                session.SaveChanges();
            }
            _updateNotifier.Notify(id.ToString(), model);
        }

    }
}