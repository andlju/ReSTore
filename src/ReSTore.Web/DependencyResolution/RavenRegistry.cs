using Raven.Client;
using Raven.Client.Document;
using ReSTore.Views;
using StructureMap.Configuration.DSL;

namespace ReSTore.Web.DependencyResolution
{
    public class RavenRegistry : Registry
    {
        public RavenRegistry()
        {
            For<IDocumentStore>().Singleton().Use(c =>
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