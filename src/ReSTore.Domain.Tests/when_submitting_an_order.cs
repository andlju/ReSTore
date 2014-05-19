using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Domain.CommandHandlers;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;
using ReSTore.Messages.Events;

namespace ReSTore.Domain.Tests
{
    [TestClass]
    public class when_submitting_an_order : with<SubmitOrder>
    {
        private readonly Guid _orderId = Guid.NewGuid();
        private readonly Guid _productId = Guid.NewGuid();

        protected override ICommandHandler<SubmitOrder> WithHandler(IRepository<Guid> repository)
        {
            return new SubmitOrderHandler(repository);
        }

        protected override void Given(IGiven given)
        {
            given.Event(_orderId, new OrderCreated() {OrderId = _orderId});
            given.Event(_orderId, new ItemAddedToOrder() { OrderId = _orderId, Price = 13.37m, ProductId = _productId });
            given.Event(_orderId, new BookingReferenceSet() {OrderId = _orderId, BookingReference = "1234" });
        }

        protected override SubmitOrder When()
        {
            return new SubmitOrder() {OrderId = _orderId };
        }

        [TestMethod]
        public void then_OrderSubmitted_event_is_published()
        {
            For(_orderId).Event<OrderSubmitted>(0);
        }

        [TestMethod]
        public void then_OrderId_in_event_is_correct()
        {
            For(_orderId).Event<OrderSubmitted>(0, e => Assert.AreEqual(_orderId, e.OrderId));
        }
    }
}