using System;

namespace ReSTore.Messages.Events
{
    public class OrderCreated
    {
        public Guid OrderId { get; set; }
    }
}