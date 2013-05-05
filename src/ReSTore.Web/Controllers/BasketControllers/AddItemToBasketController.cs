using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ReSTore.Messages.Commands;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers.BasketControllers
{
    [TypeMappedCollectionJsonFormatter(
        typeof(BasketCommandHypermediaMapper<AddItemToOrder>), 
        typeof(BasketCommandViewHypermediaMapper))]
    public class AddItemToBasketController : ApiController
    {
        private readonly ICommandDispatcher _dispatcher;

        public AddItemToBasketController(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public IEnumerable<AddItemToOrder> Get()
        {
            return Enumerable.Empty<AddItemToOrder>();
        }

        public BasketCommandView Put(Guid commandId, AddItemToOrder command)
        {
            command.CommandId = commandId;

            _dispatcher.Dispatch(command);

            return new BasketCommandView() { CommandId = commandId, BasketId = command.OrderId };
        }
    }
}