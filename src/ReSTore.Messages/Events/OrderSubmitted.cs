using System;

namespace ReSTore.Messages.Events
{
    public class OrderSubmitted
    {
        public Guid OrderId { get; set; }
    }
}