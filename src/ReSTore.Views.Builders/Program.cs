using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Raven.Client;
using Raven.Client.Document;
using ReSTore.Infrastructure;
using StructureMap;
using StructureMap.Configuration.DSL;
using Topshelf;
using Topshelf.Runtime;

namespace ReSTore.Views.Builders
{
    public interface IService
    {
        void Start();
        void Stop();
    }

    public class ViewBuilderData
    {
        public long CommitPosition { get; set; }
        public long PreparePosition { get; set; }
    }

    public class ViewBuilderService : IService
    {
        private IDocumentStore _store;
        private EventStoreConnection _eventStoreConnection;
        private readonly IEventStoreSerializer _serializer;
        private EventStoreAllCatchUpSubscription _subscription;

        public ViewBuilderService(EventStoreConnection eventStoreConnection, IEventStoreSerializer serializer,  IDocumentStore store)
        {
            _eventStoreConnection = eventStoreConnection;
            _serializer = serializer;
            _store = store;
        }

        public void Start()
        {
            ViewBuilderData mainData;
            using (var session = _store.OpenSession())
            {
                mainData = session.Load<ViewBuilderData>("main");
            }
            Position pos = Position.Start;
            if (mainData != null)
            {
                pos = new Position(mainData.CommitPosition, mainData.PreparePosition);
            }
            _subscription = _eventStoreConnection.SubscribeToAllFrom(
                pos, false, HandleEvent);
        }

        private void HandleEvent(EventStoreCatchUpSubscription sub, ResolvedEvent evt)
        {
            if (evt.Event.EventType.StartsWith("$"))
                return;

            RavenViewModelBuilder<OrderStatusModel> builder;
            builder = new RavenViewModelBuilder<OrderStatusModel>(_store, new OrderStatusModelHandler());

            builder.Build(evt.Event.EventStreamId, new[] {_serializer.Deserialize(evt.Event)});

            ViewBuilderData mainData;
            using (var session = _store.OpenSession())
            {
                mainData = session.Load<ViewBuilderData>("main");
                if (mainData == null)
                {
                    mainData = new ViewBuilderData();
                    session.Store(mainData, "main");
                }
                if (evt.OriginalPosition.HasValue)
                {
                    var pos = evt.OriginalPosition.Value;
                    mainData.CommitPosition = pos.CommitPosition;
                    mainData.PreparePosition = pos.PreparePosition;
                    session.SaveChanges();
                }
            }
        }

        public void Stop()
        {
            _subscription.Stop(TimeSpan.FromSeconds(2));
        }
    }

    public class RavenRegistry : Registry
    {
        public RavenRegistry()
        {
            For<IDocumentStore>().Use(c =>
                                          {
                                              var store =
                                                  new DocumentStore()
                                                      {
                                                          Url = "http://localhost:8080",
                                                          DefaultDatabase = "ReSTore.Views"
                                                      };
                                              store.Initialize();
                                              return store;
                                          });

        }
    }

    public class EventStoreRegistry : Registry
    {
        public EventStoreRegistry()
        {
            For<EventStoreConnection>().Singleton().Use(c =>
                                                            {
                                                                var conn = EventStoreConnection.Create();
                                                                conn.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1113));
                                                                return conn;
                                                            });
            
            For<IEventStoreSerializer>().Use<JsonEventStoreSerializer>();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container(x =>
                                              {
                                                  x.AddRegistry<RavenRegistry>();
                                                  x.AddRegistry<EventStoreRegistry>();
                                                  x.For<IService>().Use<ViewBuilderService>();
                                              });

            HostFactory.Run(x =>
            {

                x.Service<IService>(c =>
                {
                    c.ConstructUsing(() => container.GetInstance<IService>());
                    c.WhenStarted(s => s.Start());
                    
                    c.WhenStopped(s => s.Stop());
                });

                x.RunAsLocalSystem();
                x.SetServiceName("ReSTore.ViewBuilders");
                x.SetDisplayName("ReSTore View Builders service");
                x.SetDescription("Service for building the views for ReSTore");
            });
        }
    }
}
