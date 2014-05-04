using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Infrastructure;
using ReSTore.Messages.Events;
using ReSTore.Views.Builders;
using Should;

namespace ReSTore.Views.Tests.OrderStatus.Empty
{
    [TestClass]
    public class given_no_orderstatus
    {
        protected Guid OrderId = Guid.NewGuid();

        [TestMethod]
        public void when_handling_order_created()
        {
            var orderStatusBuilder = new OrderStatusModelHandler();

            OrderStatusModel model = null;
            var eventContext = new EventContext()
            {
                Event = new OrderCreated() {OrderId = OrderId},
                EventNumber = 1,
                Headers =
                    new Dictionary<string, object>()
                    {
                        {
                            "_Timestamp",
                            new DateTime(2013, 06, 24,
                                13, 37, 42,
                                17)
                        }
                    }
            };

            orderStatusBuilder.HandleAll(ref model, new[] {eventContext});

            model.ShouldNotBeNull();
            model.Status.ShouldEqual(Status.New);
        }

        [TestMethod]
        public void when_handling_item_added()
        {
            var orderStatusBuilder = new OrderStatusModelHandler();

            var model = new OrderStatusModel()
            {
                Created = new DateTime(2013, 06, 21),
                LastChange = new DateTime(2013, 06, 21),
                Status = Status.New
            };

            var eventContext = new EventContext()
            {
                Event = new ItemAddedToOrder() {OrderId = OrderId, ProductId = Guid.NewGuid()},
                EventNumber = 2,
                Headers =
                    new Dictionary<string, object>()
                    {
                        {
                            "_Timestamp",
                            new DateTime(2013, 06, 24,
                                13, 37, 42,
                                18)
                        }
                    }
            };

            orderStatusBuilder.HandleAll(ref model, new[] {eventContext});

            model.ShouldNotBeNull();
            model.LastChange.ShouldEqual(new DateTime(2013, 06, 24, 13, 37, 42, 18));
        }
    }
}
