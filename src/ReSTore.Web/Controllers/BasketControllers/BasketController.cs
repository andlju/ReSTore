using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Raven.Client;
using ReSTore.Messages.Commands;
using ReSTore.Views;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers.BasketControllers
{
    public class BasketHypermediaMapper : ICollectionJsonDocumentWriter<Basket>
    {
        public ReadDocument Write(IEnumerable<Basket> data)
        {
            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri("/api/basket", UriKind.Relative);

            var basket = data.Single();
            if (basket == null)
            {
                return doc;
            }

            foreach (var basketItem in basket.Items)
            {
                var item = basketItem.ToItem();
                item.Href = new Uri(string.Format("/api/basket/items/{0}", basketItem.ProductId), UriKind.Relative);
                item.Links.Add(new Link() { Href = new Uri(string.Format("/api/products/{0}", basketItem.ProductId), UriKind.Relative), Rel = "description", Prompt = "Product Information"});
                collection.Items.Add(item);
            }

            return doc;
        }
    }

    [TypeMappedCollectionJsonFormatter(
        typeof(BasketHypermediaMapper),
        typeof(BasketCommandViewHypermediaMapper))]
    public class BasketController : ApiController
    {
        private readonly IDocumentStore _store;
        private readonly ICommandDispatcher _dispatcher;

        public BasketController(IDocumentStore store, ICommandDispatcher dispatcher)
        {
            _store = store;
            _dispatcher = dispatcher;
        }

        public HttpResponseMessage Get()
        {
            var basketId = Guid.Parse((string)Request.Properties[BasketIdHandler.BasketIdToken]);

            using (var session = _store.OpenSession("ReSTore.Views"))
            {
                var basket = session.Load<Basket>(basketId);
                if (basket == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                return Request.CreateResponse(HttpStatusCode.OK, basket);
            }
        }

        public BasketCommandView Post()
        {
            var commandId = Guid.NewGuid();
            var basketId = Guid.Parse((string)Request.Properties[BasketIdHandler.BasketIdToken]);

            _dispatcher.Dispatch(new CreateOrder() { CommandId = commandId, OrderId = basketId});
            
            return new BasketCommandView() { CommandId = commandId, BasketId = basketId };
        }
    }
}