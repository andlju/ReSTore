using System;
using System.Collections.Generic;
using System.Web.Http;
using Raven.Client;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers
{

    public static class DataExtensions
    {
        public static Item ToItem(this Area area)
        {
            var item = new Item();
            item.Data.Add(new Data() { Name = "id", Value = area.Id.ToString() });
            item.Data.Add(new Data() { Name = "name", Value = area.Name });

            return item;
        }
    }

    public class AreaHypermediaMapper : ICollectionJsonDocumentWriter<Area>
    {
        public ReadDocument Write(IEnumerable<Area> data)
        {
            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri("http://test.com/api/areas");
            foreach (var area in data)
            {
                collection.Items.Add(area.ToItem());
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