using System;
using System.ComponentModel.DataAnnotations;

namespace ReSTore.Messages.Commands
{
    public class SetBookingReference : Command
    {
        public Guid OrderId { get; set; }

        [Display(Prompt = "Booking reference number")]
        public string BookingReference { get; set; }
    }
}