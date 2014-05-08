using System;

namespace ReSTore.Messages.Events
{
    public class ItemAddedToOrder
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public decimal Price { get; set; }
    }

}