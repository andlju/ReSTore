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

            var data = _serializer.Serialize(obj);

            Assert.AreEqual<string>("ReSTore.Infrastructure.Tests.TestClassToSerialize, ReSTore.Infrastructure.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", data.Type);
        }
         
    }
}