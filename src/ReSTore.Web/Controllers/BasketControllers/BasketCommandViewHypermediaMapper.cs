using System;
using System.Collections.Generic;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers.BasketControllers
{
    public class BasketCommandViewHypermediaMapper : 
        ICollectionJsonDocumentWriter<BasketCommandView>
    {
        public ReadDocument Write(IEnumerable<BasketCommandView> data)
        {
            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri(string.Format("/api/basket/commands"), UriKind.Relative);

            foreach (var command in data)
            {
                var item = command.ToItem();
                item.Href = new Uri(string.Format("/api/commands/{0}", command.CommandId), UriKind.Relative);
                // item.Links.Add(new Link() { Href = new Uri(string.Format("/api/products/{0}", basketItem.ItemId), UriKind.Relative), Rel = "description", Prompt = "Product Information" });
                collection.Items.Add(item);
            }

            return doc;
        }
    }
}