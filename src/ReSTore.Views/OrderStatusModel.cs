using System;

namespace ReSTore.Views
{
    public class OrderStatusModel
    {
        public Status Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastChange { get; set; }
    }
}