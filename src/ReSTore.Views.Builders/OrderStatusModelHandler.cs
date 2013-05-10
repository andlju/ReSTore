using System;
using System.Collections.Generic;
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
                        OrderItems = new List<OrderItem>()
                    };
            }
            model.OrderItems.Add(new OrderItem() { ProductId = itemAddedToOrder.ProductId, Price = itemAddedToOrder.Price });

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
            }
            return lastProcessedEvent;
        }

        public OrderStatusModel Handle(OrderStatusModel model, OrderCreated orderCreated, IDictionary<string, object> headers)
        {
            var timestamp = (DateTime)headers["_Timestamp"];
            return new OrderStatusModel() { Created = timestamp, Status = Status.New };
        }

        public OrderStatusModel Handle(OrderStatusModel model, ItemAddedToOrder itemAddedToOrder, IDictionary<string, object> headers)
        {
            var timestamp = (DateTime)headers["_Timestamp"];
            model.LastChange = timestamp;
            
            return model;
        }
    }
}