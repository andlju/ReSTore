using System;

namespace ReSTore.Messages.Commands
{
    public class AddItemToOrder
    {
        public Guid OrderId { get; set; }
        public Guid ItemId { get; set; }
    }
}