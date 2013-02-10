using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReSTore.Infrastructure.Tests
{
    [TestClass]
    public class InMemoryRepositoryTest : RepositoryTestBase
    {
        private InMemoryRepository _inMemoryRepository;

        protected override IRepository<Guid> Repository
        {
            get { return _inMemoryRepository; }
        }

        [TestMethod]
        public override void when_getting_newly_created_aggregate()
        {
            base.when_getting_newly_created_aggregate();
        }

        [TestMethod]
        public override void when_getting_previously_unstored_aggregate()
        {
            base.when_getting_previously_unstored_aggregate();
        }

        [TestMethod]
        public override void when_getting_unknown_aggregate_type()
        {
            base.when_getting_unknown_aggregate_type();
        }

        [TestMethod]
        public override void when_storing_a_new_aggregate()
        {
            base.when_storing_a_new_aggregate();
        }

        [TestInitialize]
        public void Initialize()
        {
            _inMemoryRepository = new InMemoryRepository();
        }

        protected override void FillRepository(Guid aggregateId, object[] events)
        {
            _inMemoryRepository.Store(aggregateId, events);
        }
    }
}