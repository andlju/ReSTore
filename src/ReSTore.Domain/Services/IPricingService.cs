using System;
using System.Linq;
using Raven.Client;

namespace ReSTore.Domain.Services
{
    public interface IPricingService
    {
        decimal GetPrice(Guid itemId);
    }

    public class Product
    {
        public Guid Id { get; set; }
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
                var product = session.Load<Product>(itemId.ToString());
                if (product == null)
                    return 0;

                return product.Price;
            }
        }
    }
}