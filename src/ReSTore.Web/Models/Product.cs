using System;

namespace ReSTore.Web.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public Guid AreaId { get; set; }
        public Guid CategoryId { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string ImageHref { get; set; }
        public string Amount { get; set; }
        public decimal Price { get; set; }
    }
}