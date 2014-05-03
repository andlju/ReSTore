using System;
using System.Collections.Generic;
using System.Diagnostics;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Raven.Client;
using ReSTore.Infrastructure;
using StructureMap;

namespace ReSTore.Views.Builders
{
	public class NullModelUpdateNotifier : IModelUpdateNotifier
	{
		public void Notify<TModel>(string id, TModel model)
		{

		}
	}

    public class ViewBuilderService : IService
    {
        private IDocumentStore _store;
        private IModelUpdateNotifier _updateNotifier;
        private IEventStoreConnection _eventStoreConnection;
        private IEventStoreSerializer _serializer;
        private EventStoreAllCatchUpSubscription _subscription;

		private IContainer _container;

        public ViewBuilderService(IContainer container)
        {
	        _container = container;
        }

        public void Start()
        {
			_eventStoreConnection = _container.GetInstance<IEventStoreConnection>();
			_serializer = _container.GetInstance<IEventStoreSerializer>();
			_store = _container.GetInstance<IDocumentStore>();
			_updateNotifier = new NullModelUpdateNotifier();
			// _updateNotifier = _container.GetInstance<IModelUpdateNotifier>();
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
                pos, false, HandleEvent, s =>
                {
                    
                }, (s,d,e) => { }, new UserCredentials("admin", "changeit"));
            _subscription.Start();
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

            using (var session = _store.OpenSession())
            {
                StoreViewBuilderData(evt, session);
                StoreCommandData(evt, deserializedEvent, session);
                session.SaveChanges();
            }
        }
        
        private static void StoreCommandData(ResolvedEvent evt, EventContext deserializedEvent, IDocumentSession session)
        {
            var commandId = deserializedEvent.Headers["CommandId"] as string;

            var commandModel = session.Load<CommandStatusModel>(commandId);
            if (commandModel == null)
            {
                commandModel = new CommandStatusModel() { Id = commandId };
                commandModel.Events = new List<EventStatusModel>();
                session.Store(commandModel);
            }
            commandModel.Events.Add(new EventStatusModel() { StreamId = evt.Event.EventStreamId, EventNumber = evt.Event.EventNumber, Type = evt.Event.EventType});
        }

        private static void StoreViewBuilderData(ResolvedEvent evt, IDocumentSession session)
        {
            ViewBuilderData mainData;
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
                Debug.WriteLine("Position stored");
            }
        }

        public void Stop()
        {
            _subscription.Stop(TimeSpan.FromSeconds(2));
        }
    }
}