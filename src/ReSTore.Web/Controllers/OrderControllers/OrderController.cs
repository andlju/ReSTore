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
            doc.Collection.Links.Add(new Link() { Href = new Uri(string.Format("orderhub:{0}", order.OrderId)), Rel = "orderhub" });
            
            doc.Collection.Links.Add(new Link() { Href = new Uri("/api/commands/order/setBookingReference", UriKind.Relative), Rel = "command", Prompt = "Set Booking Reference"});

            if (!string.IsNullOrWhiteSpace(order.BookingReference))
            {
                doc.Collection.Links.Add(new Link() { Href = new Uri("/api/commands/order/submitOrder", UriKind.Relative), Rel = "command", Prompt = "Submit"});
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

    [TypeMappedCollectionJsonFormatter(typeof(OrderHypermediaMapper))]
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
            
            // Get Order-Id from header
            var orderId = Guid.Parse(headers.First());

            // Try to load the order and its items
            var order = LoadOrder(orderId);
            if (order == null)
                Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, order);
        }

        public HttpResponseMessage Post()
        {
            var commandId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            _dispatcher.Dispatch(new CreateOrder() { CommandId = commandId, OrderId = orderId});

            var resp = Request.CreateResponse(HttpStatusCode.Created);
            resp.Headers.Location = new Uri(Url.Link("DefaultApi", new {controller = "order"}));
            resp.Headers.Add(OrderIdToken, orderId.ToString());

            return resp;
        }


        private Order LoadOrder(Guid orderId)
        {
            var order = new Order()
                {
                    OrderId = orderId,
                };

            using (var viewSession = _store.OpenSession("ReSTore.Views"))
            {
                var status = viewSession.Load<OrderStatusModel>("OrderStatusModel/" + orderId);
                if (status == null)
                    return null;

                var items = viewSession.Load<OrderItemsModel>("OrderItemsModel/" + orderId);
                if (items != null)
                {
                    order.Items = items.OrderItems.Select(
                        o => new OrderItem()
                            {
                                ProductId = o.ProductId,
                                Name = "Unknown",
                                Amount = o.Amount,
                                Price = o.Price
                            }).ToList();
                }
                else
                {
                    order.Items = new List<OrderItem>();
                }
                order.TotalPrice = order.Items.Aggregate(0m, (total, orderItem) => total + orderItem.Amount * orderItem.Price);
            }

            using (var catalogSession = _store.OpenSession("ReSTore"))
            {
                var productInfos = catalogSession.Load<Product>(order.Items.Select(o => o.ProductId.ToString()));
                foreach (var prod in productInfos)
                {
                    var orderItem = order.Items.Single(oi => oi.ProductId == prod.ProductId);
                    orderItem.Name = prod.Name;
                }
            }
            return order;
        }
    }
}