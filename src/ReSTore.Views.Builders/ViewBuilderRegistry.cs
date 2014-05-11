using Raven.Client;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace ReSTore.Views.Builders
{
    public class ViewBuilderRegistry : Registry
    {
        public ViewBuilderRegistry()
        {
            IncludeRegistry<RavenViewRegistry>();
            Scan(s =>
            {
                s.TheCallingAssembly();
                s.ConnectImplementationsToTypesClosing(typeof (IModelHandler<>)).OnAddedPluginTypes(c =>
                {
                    c.Singleton();
                });
            });
            For<IViewModelBuilder>().Singleton().Use<RavenViewModelBuilder<OrderItemsModel>>().Ctor<IDocumentStore>().IsNamedInstance("Views");
            For<IViewModelBuilder>().Singleton().Use<RavenViewModelBuilder<OrderStatusModel>>().Ctor<IDocumentStore>().IsNamedInstance("Views");
        }
    }
}