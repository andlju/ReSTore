using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ReSTore.Messages.Commands;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers.OrderControllers
{
    [TypeMappedCollectionJsonFormatter(
        typeof(OrderCommandHypermediaMapper<SetBookingReference>))]
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

        public HttpResponseMessage Post([FromBody]SetBookingReference command)
        {
            var commandId = Guid.NewGuid();
            command.CommandId = commandId;

            _dispatcher.Dispatch(command);

            var response = Request.CreateResponse(HttpStatusCode.Accepted);
            response.Headers.Location = new Uri(string.Format("/api/commands/{0}", commandId), UriKind.Relative);
            return response;
        }
    }
}