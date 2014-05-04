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
			// _updateNotifier = new NullModelUpdateNotifier();
			// _updateNotifier = _container.GetInstance<IModelUpdateNotifier>();

            var viewModelBuilders = _container.GetAllInstances<IViewModelBuilder>();
            foreach (var viewModelBuilder in viewModelBuilders)
            {
    			var pos = GetPosition(viewModelBuilder.GetName());
                var builder = viewModelBuilder;
                var sub = _eventStoreConnection.SubscribeToAllFrom(
                    pos,
                    false,
                    (s, resolvedEvent) => HandleEvent(builder, resolvedEvent),
                    null, null,
                    new UserCredentials("admin", "changeit"));
                sub.Start();
            }
        }

        private Position GetPosition(string modelName)
        {
            ViewBuilderData mainData;
            using (var session = _store.OpenSession())
            {
                mainData = session.Load<ViewBuilderData>(modelName);
            }
            Position pos = Position.Start;
            if (mainData != null)
            {
                pos = new Position(mainData.CommitPosition, mainData.PreparePosition);
            }
            return pos;
        }


        private void HandleEvent(IViewModelBuilder builder, ResolvedEvent evt)
        {
            if (evt.Event.EventType.StartsWith("$"))
                return;

            Debug.WriteLine("Handling event: {1} {0}", evt.Event.EventStreamId, evt.Event.EventNumber);
            var deserializedEvent = _serializer.Deserialize(evt.Event);

            builder.Build(evt.Event.EventStreamId, evt.Event.EventNumber, evt.Event.EventNumber, new[] { deserializedEvent });

            using (var session = _store.OpenSession())
            {
                StoreViewBuilderData(builder.GetName(), evt, session);
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

        private static void StoreViewBuilderData(string modelName, ResolvedEvent evt, IDocumentSession session)
        {
            ViewBuilderData mainData;
            mainData = session.Load<ViewBuilderData>(modelName);
            if (mainData == null)
            {
                mainData = new ViewBuilderData();
                session.Store(mainData, modelName);
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

        }
    }
}