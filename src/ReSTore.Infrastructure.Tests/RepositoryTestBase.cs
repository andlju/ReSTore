using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReSTore.Infrastructure.Tests
{
    public class TestAgg : Aggregate
    {
        public Guid Id;

        public TestAgg()
        {
            
        }

        public TestAgg(Guid id)
        {
            Publish(new TestCreated() { TestId = id });
        }

        private void Apply(TestCreated evt)
        {
            Id = evt.TestId;
        }
    }

    public class UnknownAgg : Aggregate
    {

        private void Apply(TestCreated evt)
        {

        }
    }

    public class TestCreated
    {
        public Guid TestId;
    }

    public class UnknownEvent
    {
        public string Test;
    }

    public class TestEventDispatcher : IEventDispatcher
    {
        public List<object> DispatchedEvents = new List<object>();
 
        public void Dispatch(IEnumerable<object> events)
        {
            DispatchedEvents.AddRange(events);    
        }
    }

    public abstract class RepositoryTestBase
    {
        protected abstract IRepository<Guid> Repository { get; }

        public virtual void when_getting_newly_created_aggregate()
        {
            var testId = Guid.NewGuid();
            FillRepository(testId, new object[] {new TestCreated() {TestId = testId}});

            var agg = Repository.GetAggregate<TestAgg>(testId);
            
            Assert.AreEqual<Guid>(testId, agg.Id);
        }

        public virtual void when_getting_previously_unstored_aggregate()
        {
            var testId = Guid.NewGuid();

            var agg = Repository.GetAggregate<TestAgg>(testId);

            Assert.IsNull(agg);
        }

        public virtual void when_getting_unknown_aggregate_type()
        {
            var testId = Guid.NewGuid();
            FillRepository(testId, new object[] { new TestCreated() { TestId = testId } });

            var agg = Repository.GetAggregate<UnknownAgg>(testId);

            Assert.IsInstanceOfType(agg, typeof(UnknownAgg));
        }

        public virtual void when_storing_a_new_aggregate()
        {
            var testId = Guid.NewGuid();
            var agg = new TestAgg(testId);

            Repository.Store(testId, agg, headers => { });

            var events = Repository.GetEvents(testId);

            Assert.IsInstanceOfType(Enumerable.First<object>(events), typeof(TestCreated));
        }

        public virtual void when_dispatcher_has_been_registered()
        {
            var testId = Guid.NewGuid();
            var agg = new TestAgg(testId);
            var testDispatcher = new TestEventDispatcher();

            Repository.RegisterDispatcher(testDispatcher);
            Repository.Store(testId, agg, headers => { });

            Assert.IsInstanceOfType(testDispatcher.DispatchedEvents.First(), typeof(TestCreated));
        }

        protected abstract void FillRepository(Guid aggregateId, object[] events);
    }
}