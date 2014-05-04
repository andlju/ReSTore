using System.Collections.Generic;
using System.Linq;
using ReSTore.Infrastructure;
using ReSTore.Messages.Events;

namespace ReSTore.Views.Builders
{
    public class OrderItemsModelHandler : IModelHandler<OrderItemsModel>
    {
        public void HandleAll(ref OrderItemsModel model, IEnumerable<EventContext> events)
        {
            foreach (var eventContext in events)
            {
                var itemAddedToOrder = eventContext.Event as ItemAddedToOrder;
                if (itemAddedToOrder != null)
                {
                    model = Handle(model, itemAddedToOrder, eventContext.Headers);
                }
            }
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
}