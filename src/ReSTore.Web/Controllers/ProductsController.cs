using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Raven.Client;
using Raven.Client.Linq;
using ReSTore.Web.Models;
using WebApiContrib.CollectionJson;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers
{
    public class ProductsHypermediaMapper : ICollectionJsonDocumentWriter<Product>
    {
        public IReadDocument Write(IEnumerable<Product> data)
        {
            var areaId = data.First().AreaId;
            var categoryId = data.First().CategoryId;

            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri(string.Format("/api/areas/{0}/categories/{1}/products", areaId, categoryId), UriKind.Relative);
            collection.Links.Add(new Link() { Href = new Uri(string.Format("/api/areas/{0}/categories", areaId), UriKind.Relative), Rel = "parent"});
            foreach (var product in data)
            {
                var item = product.ToItem();
                item.Href = new Uri(string.Format("/api/areas/{0}/categories/{1}/products/{2}", areaId, categoryId, product.ProductId), UriKind.Relative);
                
                item.Links.Add(new Link()
                    {
                        Href = new Uri(string.Format("/api/commands/order/addItemToOrder"), UriKind.Relative),
                        Rel="command",
                        Prompt = "Add to order"
                    });
                
                collection.Items.Add(item);
            }
            collection.Template.PopulateTemplate(typeof(Product));

            return doc;
        }
    }

    [TypeMappedCollectionJsonFormatter(typeof(ProductsHypermediaMapper))]
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