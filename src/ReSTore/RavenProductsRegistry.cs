using Raven.Client;
using Raven.Client.Document;
using StructureMap.Configuration.DSL;

namespace ReSTore
{
    public class RavenProductsRegistry : Registry
    {
        public RavenProductsRegistry()
        {
            For<IDocumentStore>().Singleton().Use("Create Raven Product Document store", c =>
                {
                    var store = new DocumentStore()
                        {
                            Url = "http://localhost:8080",
                            DefaultDatabase = "ReSTore"
                        };
                    store.Initialize();
                    return store;
                });
        }
    }
}