using System;
using System.Diagnostics;
using MassTransit;
using Microsoft.AspNet.SignalR;
using ReSTore.Messages.Notifications;

namespace ReSTore.Web.Controllers.OrderControllers
{
    public class OrderHub : Hub
    {
        public void RegisterForOrder(Guid orderId)
        {
            Groups.Add(Context.ConnectionId, "Order_" + orderId);
        }
    }

    public class ViewModelNotifier : Consumes<ViewModelUpdated>.All
    {
        public void Consume(ViewModelUpdated message)
        {
            Debug.WriteLine(string.Format("ViewModelUpdated consumed: {0} {1}", message.Id, message.Type));
            var hub = GlobalHost.ConnectionManager.GetHubContext<OrderHub>();
            hub.Clients.Group("Order_" + message.Id).viewModelUpdated(message.Id, message.Type, message.Content);
        }
    }
}