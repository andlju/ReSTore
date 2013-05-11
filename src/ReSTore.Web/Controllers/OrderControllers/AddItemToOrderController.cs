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
        typeof(OrderCommandHypermediaMapper<AddItemToOrder>), 
        typeof(OrderCommandViewHypermediaMapper))]
    public class AddItemToOrderController : ApiController
    {
        private readonly ICommandDispatcher _dispatcher;

        public AddItemToOrderController(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public IEnumerable<AddItemToOrder> Get()
        {
            return Enumerable.Empty<AddItemToOrder>();
        }

        public OrderCommandView Put(Guid commandId, [FromBody]AddItemToOrder command)
        {
            command.CommandId = commandId;

            _dispatcher.Dispatch(command);

            return new OrderCommandView() { CommandId = commandId, OrderId = command.OrderId };
        }
    }
}