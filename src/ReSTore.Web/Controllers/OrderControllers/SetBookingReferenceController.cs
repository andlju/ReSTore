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
        typeof(OrderCommandHypermediaMapper<SetBookingReference>),
        typeof(OrderCommandViewHypermediaMapper))]
    public class SetBookingReferenceController : ApiController
    {
        private readonly ICommandDispatcher _dispatcher;

        public SetBookingReferenceController(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public IEnumerable<SetBookingReference> Get()
        {
            return Enumerable.Empty<SetBookingReference>();
        }

        public OrderCommandView Post([FromBody]SetBookingReference command)
        {
            var commandId = Guid.NewGuid();
            command.CommandId = commandId;

            _dispatcher.Dispatch(command);

            return new OrderCommandView() { CommandId = commandId, OrderId = command.OrderId };
        }
    }
}