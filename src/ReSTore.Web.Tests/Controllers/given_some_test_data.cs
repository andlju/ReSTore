using Raven.Client;
using Raven.Client.Embedded;

namespace ReSTore.Web.Tests.Controllers
{
    public abstract class given_some_test_data
    {
        protected static IDocumentStore Store;
        
        static given_some_test_data()
        {
            Store = new EmbeddableDocumentStore()
                        {
                            RunInMemory = true
                        };
            Store.Initialize();
            using (var session = Store.OpenSession())
            {
                foreach (var area in TestDataHelpers.GetAreas())
                    session.Store(area);
                foreach(var category in TestDataHelpers.GetCategories())
                    session.Store(category);
                foreach(var product in TestDataHelpers.GetProducts())
                    session.Store(product);
                session.SaveChanges();
            }
        }
    }
}