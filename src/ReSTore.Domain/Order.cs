using System;
using ReSTore.Domain.Services;
using ReSTore.Infrastructure;
using ReSTore.Messages.Events;

namespace ReSTore.Domain
{
    public class Order : AggregateRoot
    {
        private Guid _orderId;
        private int _numberOfItems;
        private bool _hasReference;

        public Order()
        {
            
        }

        public Order(Guid orderId)
        {
            Raise(new OrderCreated() { OrderId = orderId });
        }

        public void AddItem(Guid itemId, int numberOfItems, IPricingService pricingService)
        {
            var price = pricingService.GetPrice(itemId);

            while (numberOfItems-- > 0)
            {
                Raise(new ItemAddedToOrder() { OrderId = _orderId, ProductId = itemId, Price = price });
            }
        }

        public void SetBookingReference(string bookingReference)
        {
            Raise(new BookingReferenceSet() {OrderId = _orderId, BookingReference = bookingReference});
        }

        public void Submit()
        {
            if(_numberOfItems == 0)
                throw new Exception("Order must have at least one item");
            if(!_hasReference)
                throw new Exception("Set a booking reference before submitting the order");

            Raise(new OrderSubmitted() { OrderId = _orderId });
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