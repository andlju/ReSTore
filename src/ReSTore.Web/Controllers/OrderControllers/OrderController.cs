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

namespace ReSTore.Web.Controllers.OrderControllers
{
    public class OrderHypermediaMapper : ICollectionJsonDocumentWriter<Order>
    {
        public ReadDocument Write(IEnumerable<Order> data)
        {
            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri("/api/order", UriKind.Relative);

            var order = data.Single();
            if (order == null)
            {
                return doc;
            }

            foreach (var orderItem in order.Items)
            {
                var item = orderItem.ToItem();
                item.Href = new Uri(string.Format("/api/order/items/{0}", orderItem.ProductId), UriKind.Relative);
                item.Links.Add(new Link() { Href = new Uri(string.Format("/api/products/{0}", orderItem.ProductId), UriKind.Relative), Rel = "description", Prompt = "Product Information"});
                collection.Items.Add(item);
            }

            return doc;
        }
    }

    [TypeMappedCollectionJsonFormatter(
        typeof(OrderHypermediaMapper),
        typeof(OrderCommandViewHypermediaMapper))]
    public class OrderController : ApiController
    {
        private readonly IDocumentStore _store;
        private readonly ICommandDispatcher _dispatcher;

        public static string OrderIdToken = "Order-Id";

        public OrderController(IDocumentStore store, ICommandDispatcher dispatcher)
        {
            _store = store;
            _dispatcher = dispatcher;
        }

        public HttpResponseMessage Get()
        {
            IEnumerable<string> headers;
            if (!Request.Headers.TryGetValues(OrderIdToken, out headers))
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            var orderId = Guid.Parse(headers.First());

            using (var session = _store.OpenSession("ReSTore.Views"))
            {
                var order = session.Load<OrderItemsModel>("OrderItemsModel/" + orderId);
                if (order == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                return Request.CreateResponse(HttpStatusCode.OK, order);
            }
        }

        public HttpResponseMessage Post()
        {
            var commandId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            _dispatcher.Dispatch(new CreateOrder() { CommandId = commandId, OrderId = orderId});

            var resp = Request.CreateResponse(HttpStatusCode.Created);
            resp.Headers.Location = new Uri(Url.Link("DefaultApi", new {controller = "order"}));
            resp.Headers.Add("Order-Id", orderId.ToString());

            return resp;
        }
    }
}