using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Raven.Client;
using ReSTore.Messages.Commands;
using ReSTore.Views;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers.OrderControllers
{
    [TypeMappedCollectionJsonFormatter(
            typeof(OrderCommandViewHypermediaMapper))]
    public class CommandsController : ApiController
    {
        private IDocumentStore _store;

        public CommandsController(IDocumentStore store)
        {
            _store = store;
        }

        public OrderCommandView Get(Guid id)
        {
            using (var session = _store.OpenSession("ReSTore.Views"))
            {
                var commandStatus = session.Load<CommandStatusModel>(id.ToString());
                var view = new OrderCommandView()
                    {
                        CommandId = Guid.Parse(commandStatus.Id),
                        OrderId = Guid.Parse(commandStatus.Events.First().StreamId),
                        Events =
                            commandStatus.Events.Select(
                                e => new EventView() {EventNumber = e.EventNumber, Type = e.Type}).ToList()
                    };
                return view;
            }
        }
    }
}