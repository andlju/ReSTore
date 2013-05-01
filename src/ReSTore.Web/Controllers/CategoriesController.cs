using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Raven.Client;
using Raven.Client.Linq;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers
{
    public class CategoriesHypermediaMapper : ICollectionJsonDocumentWriter<Category>
    {
        public ReadDocument Write(IEnumerable<Category> data)
        {
            var areaId = data.First().AreaId;

            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri(string.Format("/api/areas/{0}/categories", areaId), UriKind.Relative);
            
            foreach (var category in data)
            {
                var item = category.ToItem();
                item.Href = new Uri(string.Format("/api/areas/{0}/categories/{1}", areaId, category.Id), UriKind.Relative);
                collection.Items.Add(item);
            }

            return doc;
        }
    }
        
    [TypeMappedCollectionJsonFormatter(typeof(CategoriesHypermediaMapper))]
    public class CategoriesController : ApiController
    {
        private readonly IDocumentStore _store;

        public CategoriesController(IDocumentStore store) 
        {
            _store = store;
        }

        public IEnumerable<Category> Get(Guid areaId)
        {
            using (var session = _store.OpenSession())
            {
                return session.Query<Category>().Where(c => c.AreaId == areaId);
            }
        }

        public Category Get(Guid areaId, Guid id)
        {
            using (var session = _store.OpenSession())
            {
                return session.Load<Category>(id);
            }
        }
    }

    public class ProductsController : ApiController
    {
        private readonly IDocumentStore _store;

        public ProductsController(IDocumentStore store)
        {
            _store = store;
        }

        public IEnumerable<Product> Get(Guid categoryId)
        {
            using (var session = _store.OpenSession())
            {
                return session.Query<Product>().Where(p => p.CategoryId == categoryId);
            }
        }

        public Product Get(Guid areaId, Guid categoryId, Guid id)
        {
            using (var session = _store.OpenSession())
            {
                return session.Load<Product>(id);
            }
        }
    }
}