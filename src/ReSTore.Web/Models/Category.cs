using System;

namespace ReSTore.Web.Models
{
    public class Category
    {
        public Guid CategoryId { get; set; }
        public Guid AreaId { get; set; }
        public string Name { get; set; }
        public string ImageHref { get; set; }
    }
}