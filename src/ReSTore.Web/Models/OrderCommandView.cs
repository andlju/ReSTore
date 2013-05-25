using System;
using System.Collections.Generic;

namespace ReSTore.Web.Models
{
    public class OrderCommandView
    {
        public Guid CommandId { get; set; }
        public Guid OrderId { get; set; }

        public List<EventView> Events { get; set; }
    }

    public class EventView
    {
        public int EventNumber { get; set; }
        public string Type { get; set; }
    }
}