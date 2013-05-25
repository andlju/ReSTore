using System;

namespace ReSTore.Messages.Commands
{
    public class SubmitOrder : Command
    {
        public Guid OrderId { get; set; }
    }
}