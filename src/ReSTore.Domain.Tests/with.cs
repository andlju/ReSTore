using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Domain.Tests.Infrastructure;
using ReSTore.Infrastructure;

namespace ReSTore.Domain.Tests
{
    public abstract class with<T>
    {
        protected class EventPair
        {
            public Guid AggregateId;
            public object Event;
        }

        protected Guid AggregateId = Guid.NewGuid();
        protected object[] PublishedEvents;

        private MockRepository _repository;

        protected Exception ThrownException { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            _repository = new MockRepository();

            var givenEvents = Given();
            var numberOfGivenEvents = 0;
            if (givenEvents != null)
            {
                var givenEventsArr = givenEvents as object[] ?? givenEvents.ToArray();
                _repository.Store(AggregateId, givenEventsArr);
                numberOfGivenEvents = givenEventsArr.Length;
            }

            var handler = WithHandler(_repository);
            var command = When();
            try
            {
                handler.Handle(command);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
            var publishedEvents = _repository.GetEvents(AggregateId) ?? new object[0];
            PublishedEvents = publishedEvents.Skip(numberOfGivenEvents).ToArray();
        }

        protected abstract ICommandHandler<T> WithHandler(IRepository<Guid> repository);

        protected abstract IEnumerable<EventPair> Given();

        protected abstract T When();

        protected EventPair GivenEvent(Guid aggregateId, object evt)
        {
            return new EventPair() { AggregateId = aggregateId, Event = evt};
        }

        protected TEvent GetEvent<TEvent>(int eventIndex)
        {
            return ((TEvent)PublishedEvents[eventIndex]);
        }

    }
}