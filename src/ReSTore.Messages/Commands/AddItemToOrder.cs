using System;
using System.ComponentModel.DataAnnotations;

namespace ReSTore.Messages.Commands
{
    public class AddItemToOrder : Command
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        [Display(Prompt = "Number of items")]
        public int NumberOfItems { get; set; }
    }
}