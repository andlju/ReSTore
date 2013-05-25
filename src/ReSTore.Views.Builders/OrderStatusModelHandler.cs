using System;
using System.Collections.Generic;
using System.Linq;
using ReSTore.Infrastructure;
using ReSTore.Messages.Events;

namespace ReSTore.Views.Builders
{
    public interface IModelHandler<TModel>
    {
        int HandleAll(ref TModel model, IEnumerable<EventContext> events);
    }

    public class OrderItemsModelHandler : IModelHandler<OrderItemsModel>
    {
        public int HandleAll(ref OrderItemsModel model, IEnumerable<EventContext> events)
        {
            var lastProcessedEvent = 0;
            foreach (var eventContext in events)
            {
                lastProcessedEvent = eventContext.EventNumber;
                var itemAddedToOrder = eventContext.Event as ItemAddedToOrder;
                if (itemAddedToOrder != null)
                {
                    model = Handle(model, itemAddedToOrder, eventContext.Headers);
                    continue;
                }
            }
            return lastProcessedEvent;
        }

        public OrderItemsModel Handle(OrderItemsModel model, ItemAddedToOrder itemAddedToOrder, IDictionary<string, object> headers)
        {
            if (model == null)
            {
                model = new OrderItemsModel()
                    {
                        Id = itemAddedToOrder.OrderId.ToString(),
                        OrderItems = new List<OrderItemModel>()
                    };
            }
            var orderItem =
                model.OrderItems.FirstOrDefault(
                    oi => oi.ProductId == itemAddedToOrder.ProductId && oi.Price == itemAddedToOrder.Price);

            if (orderItem == null)
            {
                orderItem = new OrderItemModel()
                    {
                        ProductId = itemAddedToOrder.ProductId,
                        Price = itemAddedToOrder.Price,
                        Amount = 1
                    };
                model.OrderItems.Add(orderItem);
            }
            else
            {
                orderItem.Amount += 1;
            }

            return model;
        }
    }

    public class OrderStatusModelHandler : IModelHandler<OrderStatusModel>
    {

        public int HandleAll(ref OrderStatusModel model, IEnumerable<EventContext> events)
        {
            var lastProcessedEvent = 0;
            foreach (var eventContext in events)
            {
                lastProcessedEvent = eventContext.EventNumber;
                var orderCreated = eventContext.Event as OrderCreated;
                if (orderCreated != null)
                {
                    model = Handle(model, orderCreated, eventContext.Headers);
                    continue;
                }
                var itemAddedToOrder = eventContext.Event as ItemAddedToOrder;
                if (itemAddedToOrder != null)
                {
                    model = Handle(model, itemAddedToOrder, eventContext.Headers);
                    continue;
                }
                var orderSubmitted = eventContext.Event as OrderSubmitted;
                if (orderSubmitted != null)
                {
                    model = Handle(model, orderSubmitted, eventContext.Headers);
                    continue;
                }
                var bookingReferenceSet = eventContext.Event as BookingReferenceSet;
                if (bookingReferenceSet != null)
                {
                    model = Handle(model, bookingReferenceSet, eventContext.Headers);
                    continue;
                }
            }
            return lastProcessedEvent;
        }

        public OrderStatusModel Handle(OrderStatusModel model, OrderCreated orderCreated,
                                       IDictionary<string, object> headers)
        {
            var timestamp = (DateTime) headers["_Timestamp"];
            return new OrderStatusModel()
                {
                    Id = orderCreated.OrderId.ToString(),
                    Created = timestamp,
                    Status = Status.New
                };
        }

        public OrderStatusModel Handle(OrderStatusModel model, ItemAddedToOrder itemAddedToOrder,
                                       IDictionary<string, object> headers)
        {
            var timestamp = (DateTime) headers["_Timestamp"];
            model.LastChange = timestamp;

            return model;
        }

        public OrderStatusModel Handle(OrderStatusModel model, OrderSubmitted orderSubmitted,
                                       IDictionary<string, object> headers)
        {
            var timestamp = (DateTime) headers["_Timestamp"];
            model.LastChange = timestamp;
            model.Status = Status.Submitted;
            return model;
        }

        public OrderStatusModel Handle(OrderStatusModel model, BookingReferenceSet bookingReferenceSet,
                                       IDictionary<string, object> headers)
        {
            var timestamp = (DateTime) headers["_Timestamp"];
            model.LastChange = timestamp;
            model.BookingReference = bookingReferenceSet.BookingReference;
            return model;
        }
    }
}