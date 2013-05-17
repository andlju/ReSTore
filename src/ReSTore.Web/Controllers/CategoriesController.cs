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
            var categories = data as Category[] ?? data.ToArray();
            var areaId = categories.First().AreaId;

            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri(string.Format("/api/areas/{0}/categories", areaId), UriKind.Relative);
            
            foreach (var category in categories)
            {
                var item = category.ToItem();
                item.Href = new Uri(string.Format("/api/areas/{0}/categories/{1}", areaId, category.CategoryId), UriKind.Relative);
                item.Links.Add(new Link() { Href = new Uri(string.Format("/api/areas/{0}/categories/{1}/products", areaId, category.CategoryId), UriKind.Relative), Rel = "children", Prompt = "Products" });
                collection.Items.Add(item);
            }
            collection.Template.PopulateTemplate(typeof(Category));
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
}