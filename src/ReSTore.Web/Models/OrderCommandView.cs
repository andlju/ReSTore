using System;

namespace ReSTore.Web.Models
{
    public class OrderCommandView
    {
        public Guid CommandId { get; set; }
        public Guid OrderId { get; set; }
    }
}