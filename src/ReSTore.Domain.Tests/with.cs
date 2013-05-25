using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Domain.Tests.Infrastructure;
using ReSTore.Infrastructure;

namespace ReSTore.Domain.Tests
{
    public interface IGiven
    {
        void Event(Guid aggregateId, object @event);
    }

    public abstract class with<TCommand>
    {
        class GivenImpl : IGiven
        {
            private MockRepository _repository;

            public GivenImpl(MockRepository repository)
            {
                _repository = repository;
            }

            public void Event(Guid aggregateId, object @event)
            {
                _repository.Store(aggregateId, new [] { @event }, headers => { });
            }
        }

        protected Guid AggregateId = Guid.NewGuid();
        protected object[] PublishedEvents;

        private MockRepository _repository;

        protected Exception ThrownException { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            _repository = new MockRepository();

            var given = new GivenImpl(_repository);
            Given(given);

            _repository.ResetCommitted();

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
        }

        protected abstract ICommandHandler<TCommand> WithHandler(IRepository<Guid> repository);

        protected abstract void Given(IGiven given);

        protected abstract TCommand When();

        protected EventAssertions For(Guid aggregateId)
        {
            return new EventAssertions(_repository.GetCommittedEvents(aggregateId));
        }

        protected class EventAssertions
        {
            private readonly object[] _events;

            public EventAssertions(IEnumerable<object> events)
            {
                _events = events.ToArray();
            }

            public int NumberOfEvents
            {
                get { return _events.Length; }
            }

            public TEvent GetEvent<TEvent>(int eventIndex)
            {
                return ((TEvent)_events[eventIndex]);
            }

            public void Event<TEvent>(int index)
            {
                Assert.IsInstanceOfType(_events[index], typeof(TEvent));
            }

            public void Event<TEvent>(int index, Action<TEvent> eventAssertion)
            {
                eventAssertion((TEvent)_events[index]);
            }
            
        }
    }
}