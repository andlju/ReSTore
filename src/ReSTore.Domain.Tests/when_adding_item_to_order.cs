using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Domain.CommandHandlers;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;
using ReSTore.Messages.Events;

namespace ReSTore.Domain.Tests
{
    [TestClass]
    public class when_adding_item_to_order : with<AddItemToOrder>
    {
        private Guid _orderId = Guid.NewGuid();
        private Guid _itemId = Guid.NewGuid();

        protected override ICommandHandler<AddItemToOrder> WithHandler(IRepository<Guid> repository)
        {
            return new AddItemToOrderHandler(repository);
        }

        protected override void Given(IGiven given)
        {
            given.Event(_orderId, new OrderCreated() {OrderId = _orderId});
        }

        protected override AddItemToOrder When()
        {
            return new AddItemToOrder() { OrderId = _orderId, ItemId = _itemId };
        }

        [TestMethod]
        public void then_ItemAddedToOrder_event_is_published()
        {
            For(_orderId).Event<ItemAddedToOrder>(0);
        }

        [TestMethod]
        public void then_OrderId_in_event_is_correct()
        {
            For(_orderId).Event<ItemAddedToOrder>(0, e => Assert.AreEqual(_orderId, e.OrderId));
        }

        [TestMethod]
        public void then_ItemId_in_event_is_correct()
        {
            For(_orderId).Event<ItemAddedToOrder>(0, e => Assert.AreEqual(_itemId, e.ItemId));
        }
    }
}