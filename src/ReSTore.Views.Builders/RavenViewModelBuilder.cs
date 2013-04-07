using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using ReSTore.Infrastructure;

namespace ReSTore.Views.Builders
{
    public class RavenViewModelBuilder<TModel> : IViewModelBuilder
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
            using (var session = _store.OpenSession())
            {
                var model = session.Load<TModel>(id.ToString());
                var eventNumber = _handler.HandleAll(ref model, events);
                
                session.Store(model, id.ToString());
                
                var metaData = session.Advanced.GetMetadataFor(model);
                metaData["EventNumber"] = eventNumber;

                session.SaveChanges();
            }
        }

    }
}