using System;
using System.Collections.Generic;

namespace ReSTore.Web.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public string BookingReference { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderItem
    {
        public Guid ProductId { get; set; }

        public string Name { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }
}