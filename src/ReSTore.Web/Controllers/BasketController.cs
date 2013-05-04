using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using Raven.Client;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;
using ReSTore.Web.Models;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers
{
    public class BasketItemHypermediaMapper : ICollectionJsonDocumentWriter<Basket>
    {
        public ReadDocument Write(IEnumerable<Basket> data)
        {
            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri("/api/basket", UriKind.Relative);
            
            var basket = data.Single();
            foreach (var basketItem in basket.Items)
            {
                var item = basketItem.ToItem();
                item.Href = new Uri(string.Format("/api/basket/items/{0}", basketItem.ItemId), UriKind.Relative);
                item.Links.Add(new Link() { Href = new Uri(string.Format("/api/products/{0}", basketItem.ItemId), UriKind.Relative), Rel = "description", Prompt = "Product Information"});
                collection.Items.Add(item);
            }

            return doc;
        }
    }

    public class BasketCommandsHypermediaMapper : 
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

    public class CommandHypermediaMapper<TCmd> : 
        ICollectionJsonDocumentWriter<TCmd>,
        ICollectionJsonDocumentReader<TCmd> 
        where TCmd : new()
    {
        public TCmd Read(WriteDocument document)
        {
            var command = document.Template.FromTemplate<TCmd>();
            
            return command;
        }

        public ReadDocument Write(IEnumerable<TCmd> data)
        {
            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri(string.Format("/api/basket/commands/TODO"), UriKind.Relative);
            collection.Template.PopulateTemplate(typeof(TCmd));
            
            return doc;
        }
    }

    [TypeMappedCollectionJsonFormatter(
        typeof(BasketItemHypermediaMapper),
        typeof(BasketCommandsHypermediaMapper))]
    public class BasketController : ApiController
    {
        private readonly IDocumentStore _store;

        public BasketController(IDocumentStore store)
        {
            _store = store;
        }

        public Basket Get(Guid id)
        {
            using (var session = _store.OpenSession())
            {
                return session.Load<Basket>(id);
            }
        }

    }
    
    [TypeMappedCollectionJsonFormatter(
        typeof(CommandHypermediaMapper<AddItemToOrder>), 
        typeof(BasketCommandsHypermediaMapper))]
    public class AddItemToBasketCommandController : ApiController
    {
        private readonly ICommandDispatcher _dispatcher;

        public AddItemToBasketCommandController(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public IEnumerable<AddItemToOrder> Get()
        {
            return Enumerable.Empty<AddItemToOrder>();
        }

        public BasketCommandView Put(Guid commandId, AddItemToOrder command)
        {
            command.CommandId = commandId;

            _dispatcher.Dispatch(commandId, command);

            return new BasketCommandView() { CommandId = commandId, BasketId = command.OrderId };
        }
    }
}