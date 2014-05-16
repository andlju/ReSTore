using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Domain.CommandHandlers;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;

// ReSharper disable once InconsistentNaming
namespace ReSTore.Domain.Tests
{
    [TestClass]
    public class when_adding_item_to_order_that_doesnt_exist : with<AddItemToOrder>
    {
        private readonly Guid _orderId = Guid.NewGuid();
        private readonly Guid _itemId = Guid.NewGuid();

        protected override ICommandHandler<AddItemToOrder> WithHandler(IRepository<Guid> repository)
        {
            return new AddItemToOrderHandler(repository, null);
        }

        protected override void Given(IGiven given)
        {
            
        }

        protected override AddItemToOrder When()
        {
            return new AddItemToOrder() { OrderId = _orderId, ProductId = _itemId};
        }

        [TestMethod]
        public void then_InvalidOperationException_is_thrown()
        {
            Assert.IsInstanceOfType(ThrownException, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void then_no_events_are_published()
        {
            Assert.AreEqual(0, For(_orderId).NumberOfEvents);
        }
    }
}