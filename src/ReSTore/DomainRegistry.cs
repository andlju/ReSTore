using System;
using Raven.Client;
using ReSTore.Domain;
using ReSTore.Domain.CommandHandlers;
using ReSTore.Domain.Services;
using ReSTore.Infrastructure;
using ReSTore.Views.Builders;
using StructureMap.Configuration.DSL;

namespace ReSTore
{
    public class PricingServiceRegistry : Registry
    {
        public PricingServiceRegistry()
        {
            IncludeRegistry<RavenProductsRegistry>();
            For<IPricingService>().Singleton().Use<PricingService>().Ctor<IDocumentStore>().IsNamedInstance("Products");
        }
    }

    public class DomainRegistry : Registry
    {
        public DomainRegistry()
        {
            IncludeRegistry<PricingServiceRegistry>();
            IncludeRegistry<ViewBuilderRegistry>();
            Scan(s =>
                {
                    s.AssemblyContainingType<CreateOrderHandler>();
                    s.ConnectImplementationsToTypesClosing(typeof (ICommandHandler<>));
                });
            For<IModelUpdateNotifier>().Use<NullModelUpdateNotifier>();
            For<IRepository<Guid>>().Singleton().Use<EventStoreRepository>().OnCreation("Registering ViewBuilders",
                (ctxt, repository) =>
                {
                    // repository.RegisterDispatcher(ctxt.GetInstance<ViewBuilderEventDispatcher>());
                });
        }
    }
}