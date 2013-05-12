using System;
using EasyNetQ;
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

    public class ViewModelNotifier : IConsume<ViewModelUpdated>
    {
        public void Consume(ViewModelUpdated message)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<OrderHub>();
            hub.Clients.Group("Order_" + message.Id).viewModelUpdated(message.Id, message.Type, message.Content);
        }
    }
}