using System;
using System.Collections.Generic;

namespace ReSTore.Views
{
    public class OrderStatusModel
    {
        public string Id { get; set; }
        public Status Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastChange { get; set; }
    }

    public class OrderItemsModel
    {
        public string Id { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public Guid ProductId { get; set; }

        public decimal Price { get; set; }
    }
}