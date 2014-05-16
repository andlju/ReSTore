using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Domain.CommandHandlers;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;
using ReSTore.Messages.Events;
// ReSharper disable once InconsistentNaming

namespace ReSTore.Domain.Tests
{
    [TestClass]
    public class when_creating_order : with<CreateOrder>
    {
        private readonly Guid _orderId = Guid.NewGuid();

        protected override ICommandHandler<CreateOrder> WithHandler(IRepository<Guid> repository)
        {
            return new CreateOrderHandler(repository);
        }

        protected override void Given(IGiven given)
        {
            
        }

        protected override CreateOrder When()
        {
            return new CreateOrder() { OrderId = _orderId };
        }

        [TestMethod]
        public void then_OrderCreated_event_is_published()
        {
            For(_orderId).Event<OrderCreated>(0);
        }

        [TestMethod]
        public void then_OrderId_in_event_is_correct()
        {
            For(_orderId).Event<OrderCreated>(0, e => Assert.AreEqual(_orderId, e.OrderId) );
        }
    }
}