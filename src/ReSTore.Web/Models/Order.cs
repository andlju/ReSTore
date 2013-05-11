using System;
using System.Collections.Generic;

namespace ReSTore.Web.Models
{
    public class OrderCommandView
    {
        public Guid CommandId { get; set; }
        public Guid OrderId { get; set; }
    }
    
    public class Order
    {
        public Guid OrderId { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}