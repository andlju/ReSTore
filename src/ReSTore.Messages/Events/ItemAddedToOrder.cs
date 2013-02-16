using System;

namespace ReSTore.Messages.Events
{
    public class ItemAddedToOrder
    {
        public Guid OrderId { get; set; }
        public Guid ItemId { get; set; }
    }
}