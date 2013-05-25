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
        typeof(OrderCommandHypermediaMapper<SubmitOrder>))]
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

        public HttpResponseMessage Post([FromBody]SubmitOrder command)
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