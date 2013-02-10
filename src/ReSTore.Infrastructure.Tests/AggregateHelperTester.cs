using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReSTore.Infrastructure.Tests
{
    [TestClass]
    public class AggregateHelperTester
    {

        [TestMethod]
        public void when_building_an_aggregate()
        {
            var testId = Guid.NewGuid();
            var agg = AggregateHelper.Build<TestAgg>(new object[] { new TestCreated() { TestId = testId } });

            Assert.IsInstanceOfType(agg, typeof(TestAgg));
            Assert.AreEqual<Guid>(testId, agg.Id);
        }

        [TestMethod]
        public void when_building_a_known_aggregate_with_unknown_event()
        {
            var testId = Guid.NewGuid();
            var agg = AggregateHelper.Build<TestAgg>(new object[] { new TestCreated() { TestId = testId }, new UnknownEvent() { Test = "Just testing" } });
            Assert.IsNotNull(agg);
        }
    }
}