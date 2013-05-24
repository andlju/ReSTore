using System;
using System.Collections.Generic;
using System.Web.Http;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers
{
    public class RootHypermediaMapper : ICollectionJsonDocumentWriter<Root>
    {
        public ReadDocument Write(IEnumerable<Root> data)
        {
            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri("/api", UriKind.Relative);
            collection.Links.Add(new Link() { Href = new Uri("/api/order", UriKind.Relative), Rel = "order" });
            collection.Links.Add(new Link() { Href = new Uri("/api/areas", UriKind.Relative), Rel = "children" });

            return doc;
        }
    }

    [TypeMappedCollectionJsonFormatter(typeof(RootHypermediaMapper))]
    public class RootController : ApiController
    {
        public Root Get()
        {
            return new Root();
        }
    }
}