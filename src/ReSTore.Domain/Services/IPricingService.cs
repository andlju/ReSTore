using System;
using Raven.Client;

namespace ReSTore.Domain.Services
{
    public interface IPricingService
    {
        decimal GetPrice(Guid itemId);
    }

    public class Item
    {
        public Guid ItemId { get; set; }
        public decimal Price { get; set; }
    }

    public class PricingService : IPricingService
    {
        private readonly IDocumentStore _store;

        public PricingService(IDocumentStore store)
        {
            _store = store;
        }

        public decimal GetPrice(Guid itemId)
        {
            using (var session = _store.OpenSession())
            {
                var item = session.Load<Item>(itemId);
                if (item == null)
                    return 0;

                return item.Price;
            }
        }
    }
}