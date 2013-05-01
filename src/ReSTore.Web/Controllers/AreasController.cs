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
    public class CommandView
    {
        public Guid CommandId { get; set; }
    }

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
                item.Href = new Uri(string.Format("/api/areas/{0}/categories", area.Id), UriKind.Relative);
                collection.Items.Add(item);
            }

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
    }
}