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
    public class when_creating_order : with<CreateOrder>
    {
        protected override ICommandHandler<CreateOrder> WithHandler(IRepository<Guid> repository)
        {
            return new CreateOrderHandler(repository);
        }

        protected override IEnumerable<EventPair> Given()
        {
            yield break;
        }

        protected override CreateOrder When()
        {
            return new CreateOrder() { OrderId = AggregateId };
        }

        [TestMethod]
        public void then_OrderCreated_event_is_published()
        {
            Assert.IsInstanceOfType(PublishedEvents[0], typeof(OrderCreated));
        }
    }
}