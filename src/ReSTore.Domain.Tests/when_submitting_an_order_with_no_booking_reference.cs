using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Domain.CommandHandlers;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;
using ReSTore.Messages.Events;

namespace ReSTore.Domain.Tests
{
    [TestClass]
    public class when_submitting_an_order_with_no_booking_reference : with<SubmitOrder>
    {
        private readonly Guid _orderId = Guid.NewGuid();
        private readonly Guid _productId = Guid.NewGuid();

        protected override ICommandHandler<SubmitOrder> WithHandler(IRepository<Guid> repository)
        {
            return new SubmitOrderHandler(repository);
        }

        protected override void Given(IGiven given)
        {
            given.Event(_orderId, new OrderCreated() { OrderId = _orderId });
            given.Event(_orderId, new ItemAddedToOrder() { OrderId = _orderId, Price = 13.37m, ProductId = _productId });
        }

        protected override SubmitOrder When()
        {
            return new SubmitOrder() { OrderId = _orderId };
        }

        [TestMethod]
        public void then_no_event_is_published()
        {
            Assert.AreEqual(0, For(_orderId).NumberOfEvents);
        }

        [TestMethod]
        public void then_Exception_is_thrown()
        {
            Assert.IsInstanceOfType(ThrownException, typeof(Exception));
        }
    }
}