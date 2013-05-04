using System;
using System.Collections.Generic;

namespace ReSTore.Web.Models
{
    public class BasketCommandView
    {
        public Guid CommandId { get; set; }
        public Guid BasketId { get; set; }
    }
    
    public class Basket
    {
        public Guid Id { get; set; }
        public List<BasketItem> Items { get; set; }
    }

    public class BasketItem
    {
        public Guid ItemId { get; set; }

        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}