using System;

namespace ReSTore.Messages.Commands
{
    public class CreateOrder : Command
    {
        public Guid OrderId { get; set; }
    }
}