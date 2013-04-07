using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReSTore.Infrastructure.Tests
{

    public class TestClassToSerialize
    {
        public Guid Id { get; set; }
        public string MyString { get; set; }
        public int MyInt { get; set; }
    }

    [TestClass]
    public class EventStoreSerializerTest
    {
        private IEventStoreSerializer _serializer;

        [TestInitialize]
        public void Initialize()
        {
            _serializer = new JsonEventStoreSerializer();
        }

        [TestMethod]
        public void when_serializing_simple_class()
        {
            var obj = new TestClassToSerialize()
                          {
                              Id = Guid.NewGuid(),
                              MyInt = 17,
                              MyString = "Test"
                          };

            var data = _serializer.Serialize(obj, null);

            Assert.AreEqual<string>("TestClassToSerialize", data.Type);
        }
         
    }
}