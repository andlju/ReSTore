using Raven.Client;
using Raven.Client.Document;
using StructureMap.Configuration.DSL;

namespace ReSTore.Views.Builders
{
    public class RavenViewRegistry : Registry
    {
        public RavenViewRegistry()
        {
            For<IDocumentStore>().Use("Create Raven Views Document Store", c =>
                {
                    var store =
                        new DocumentStore()
                            {
                                Url = "http://localhost:8080",
                                DefaultDatabase = "ReSTore.Views",
                            };
                    store.Conventions.ShouldCacheRequest = (url) => false;
                                              
                    store.Initialize();
                    return store;
                });

        }
    }
}