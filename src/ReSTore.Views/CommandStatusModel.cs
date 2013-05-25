using System;
using System.Collections.Generic;

namespace ReSTore.Views
{
    public class CommandStatusModel
    {
        public string Id { get; set; }

        public List<EventStatusModel> Events { get; set; }
    }

    public class EventStatusModel
    {
        public string StreamId { get; set; }
        public int EventNumber { get; set; }
        public string Type { get; set; }
    }

}