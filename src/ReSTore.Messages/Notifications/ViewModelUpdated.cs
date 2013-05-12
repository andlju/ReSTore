using System;

namespace ReSTore.Messages.Notifications
{
    public class ViewModelUpdated
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        
        public object Content { get; set; }
    }
}