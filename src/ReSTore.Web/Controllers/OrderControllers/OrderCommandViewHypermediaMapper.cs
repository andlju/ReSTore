using System;
using System.Collections.Generic;
using System.Linq;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers.OrderControllers
{
    public class OrderCommandViewHypermediaMapper : 
        ICollectionJsonDocumentWriter<OrderCommandView>
    {
        public ReadDocument Write(IEnumerable<OrderCommandView> data)
        {
            var commandView = data.Single();

            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri(string.Format("/api/commands/{0}", commandView.CommandId), UriKind.Relative);
            
            if (commandView.Events == null)
                return doc;

            foreach (var evt in commandView.Events)
            {
                var item = evt.ToItem();
                item.Href = new Uri(string.Format("/api/events/{0}/{1}", commandView.OrderId, evt.EventNumber), UriKind.Relative);
                // item.Links.Add(new Link() { Href = new Uri(string.Format("/api/products/{0}", orderItem.ItemId), UriKind.Relative), Rel = "description", Prompt = "Product Information" });
                collection.Items.Add(item);
            }

            return doc;
        }
    }
}