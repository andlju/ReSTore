using System;
using System.Diagnostics;
using EventStore.ClientAPI;
using Raven.Client;
using ReSTore.Infrastructure;

namespace ReSTore.Views.Builders
{
    public class ViewBuilderService : IService
    {
        private IDocumentStore _store;
        private readonly IModelUpdateNotifier _updateNotifier;
        private EventStoreConnection _eventStoreConnection;
        private readonly IEventStoreSerializer _serializer;
        private EventStoreAllCatchUpSubscription _subscription;

        public ViewBuilderService(EventStoreConnection eventStoreConnection, IEventStoreSerializer serializer,  IDocumentStore store, IModelUpdateNotifier updateNotifier)
        {
            _eventStoreConnection = eventStoreConnection;
            _serializer = serializer;
            _store = store;
            _updateNotifier = updateNotifier;
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

            Debug.WriteLine("Handling event: {1} {0}", evt.Event.EventStreamId, evt.Event.EventNumber);
            var deserializedEvent = _serializer.Deserialize(evt.Event);

            var orderStatusbuilder = new RavenViewModelBuilder<OrderStatusModel>(_store, new OrderStatusModelHandler(), _updateNotifier);
            orderStatusbuilder.Build(evt.Event.EventStreamId, new[] { deserializedEvent });

            var orderItemsBuilder = new RavenViewModelBuilder<OrderItemsModel>(_store, new OrderItemsModelHandler(), _updateNotifier);
            orderItemsBuilder.Build(evt.Event.EventStreamId, new[] { deserializedEvent });

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
                    Debug.WriteLine("Position stored");
                }
            }
        }

        public void Stop()
        {
            _subscription.Stop(TimeSpan.FromSeconds(2));
        }
    }
}