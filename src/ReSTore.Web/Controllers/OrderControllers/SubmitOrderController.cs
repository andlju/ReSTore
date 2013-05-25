using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ReSTore.Messages.Commands;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers.OrderControllers
{
    [TypeMappedCollectionJsonFormatter(
        typeof(OrderCommandHypermediaMapper<SubmitOrder>),
        typeof(OrderCommandViewHypermediaMapper))]
    public class SubmitOrderController : ApiController
    {
        private readonly ICommandDispatcher _dispatcher;

        public SubmitOrderController(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public IEnumerable<SubmitOrder> Get()
        {
            return Enumerable.Empty<SubmitOrder>();
        }

        public OrderCommandView Post([FromBody]SubmitOrder command)
        {
            var commandId = Guid.NewGuid();
            command.CommandId = commandId;

            _dispatcher.Dispatch(command);

            return new OrderCommandView() { CommandId = commandId, OrderId = command.OrderId };
        }
    }
}