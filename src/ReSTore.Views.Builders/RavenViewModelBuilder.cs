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

        public RavenViewModelBuilder(IDocumentStore store, IModelHandler<TModel> handler)
        {
            _store = store;
            _handler = handler;
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
            }
        }

    }
}