using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ReSTore.Web.Models;

namespace ReSTore.Web.Controllers
{
    public class ProductsApiController : ApiController
    {
        public List<ProductItem> Get()
        {
            var items = GetItems().ToList();
            return items;
        }

        private IEnumerable<ProductItem> GetItems()
        {
            yield return new ProductItem() { Brand = "Anthon Berg", Name = "Cognacmarsipan", Price = 49.00m, Description = "Klassiska marsipanbröd med cognac och mörk choklad från Anthon Berg." };
        }
    }
}