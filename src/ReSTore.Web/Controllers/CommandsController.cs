using System;
using System.Web.Http;
using ReSTore.Messages.Commands;

namespace ReSTore.Web.Controllers
{
    public class CommandsController : ApiController
    {
        private readonly ICommandDispatcher _dispatcher;

        public CommandsController(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [ActionName("create-order")]
        public CommandView Put(Guid commandId, CreateOrder command)
        {
            command.CommandId = commandId;

            _dispatcher.Dispatch(commandId, command);

            return new CommandView() { CommandId = commandId };
        }

        [ActionName("add-item-to-order")]
        public CommandView Put(Guid commandId, AddItemToOrder command)
        {
            command.CommandId = commandId;

            _dispatcher.Dispatch(commandId, command);

            return new CommandView() { CommandId = commandId };
        }
    }

}