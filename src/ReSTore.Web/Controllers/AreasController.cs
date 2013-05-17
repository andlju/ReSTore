using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using Raven.Client;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers
{

    public class AreaHypermediaMapper : ICollectionJsonDocumentWriter<Area>
    {
        public ReadDocument Write(IEnumerable<Area> data)
        {
            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri("/api/areas", UriKind.Relative);
            foreach (var area in data)
            {
                var item = area.ToItem();
                item.Href = new Uri(string.Format("/api/areas/{0}", area.AreaId), UriKind.Relative);
                item.Links.Add(new Link() { Href = new Uri(string.Format("/api/areas/{0}/categories", area.AreaId), UriKind.Relative), Rel = "children", Prompt = "Categories"});
                collection.Items.Add(item);
            }
            collection.Template.PopulateTemplate(typeof(Area));
            return doc;
        }
    }

    [TypeMappedCollectionJsonFormatter(typeof(AreaHypermediaMapper))]
    public class AreasController : ApiController
    {
        private readonly IDocumentStore _store;

        public AreasController(IDocumentStore store) 
        {
            _store = store;
        }

        public IEnumerable<Area> Get()
        {
            using (var session = _store.OpenSession())
            {
                return session.Query<Area>();
            }
        }

        public Area Get(Guid id)
        {
            using (var session = _store.OpenSession())
            {
                return session.Load<Area>(id);
            }
        }
    }
}