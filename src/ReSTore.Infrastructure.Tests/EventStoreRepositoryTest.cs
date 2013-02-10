using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReSTore.Infrastructure.Tests
{
    [TestClass]
    public class EventStoreRepositoryTest : RepositoryTestBase
    {
        private EventStoreRepository<Guid> _eventStoreRepository;

        protected override IRepository<Guid> Repository
        {
            get { return _eventStoreRepository; }
        }

        protected override void FillRepository(Guid aggregateId, object[] events)
        {
            _eventStoreRepository.Store(aggregateId, events);
        }

        [TestInitialize]
        public void Initialize()
        {
            _eventStoreRepository = new EventStoreRepository<Guid>();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public override void when_getting_newly_created_aggregate()
        {
            base.when_getting_newly_created_aggregate();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public override void when_getting_previously_unstored_aggregate()
        {
            base.when_getting_previously_unstored_aggregate();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public override void when_getting_unknown_aggregate_type()
        {
            base.when_getting_unknown_aggregate_type();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public override void when_storing_a_new_aggregate()
        {
            base.when_storing_a_new_aggregate();
        }
    }
}