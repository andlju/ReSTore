using System;
using ReSTore.Domain.Services;
using ReSTore.Infrastructure;
using ReSTore.Messages.Events;

namespace ReSTore.Domain
{
    public class Order : Aggregate
    {
        private Guid _orderId;
        private bool _hasReference;
        private int _numberOfItems;

        public Order()
        {
            
        }

        public Order(Guid orderId)
        {
            Publish(new OrderCreated() { OrderId = orderId });
        }

        public void AddItem(Guid itemId, int numberOfItems, IPricingService pricingService)
        {
            var price = pricingService.GetPrice(itemId);

            while (numberOfItems-- > 0)
            {
                Publish(new ItemAddedToOrder() { OrderId = _orderId, ProductId = itemId, Price = price });
            }
        }

        public void SetBookingReference(string bookingReference)
        {
            Publish(new BookingReferenceSet() {OrderId = _orderId, BookingReference = bookingReference});
        }

        private void Apply(OrderCreated evt)
        {
            _orderId = evt.OrderId;
        }

        private void Apply(ItemAddedToOrder evt)
        {
            _numberOfItems++;
        }

        private void Apply(BookingReferenceSet evt)
        {
            _hasReference = true;
        }
    }
}