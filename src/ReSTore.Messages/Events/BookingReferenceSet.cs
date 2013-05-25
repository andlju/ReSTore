using System;

namespace ReSTore.Messages.Events
{
    public class BookingReferenceSet
    {
        public Guid OrderId { get; set; }
        public string BookingReference { get; set; }
    }
}