using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Should;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Tests.Formatters
{
    public class TestOutputOnly
    {
        public string StringProperty { get; set; }
        public int IntegerProperty { get; set; }
    }

    public class TestInputOnly
    {
        public string StringProperty { get; set; }
        public int IntegerProperty { get; set; }
    }

    public class TestInputAndOutput
    {
        public string StringProperty { get; set; }
        public int IntegerProperty { get; set; }
    }

    public class TestOutputWriter : ICollectionJsonDocumentWriter<TestOutputOnly>
    {
        public ReadDocument Write(IEnumerable<TestOutputOnly> data)
        {
            var doc = new ReadDocument();
            foreach (var dataItem in data)
            {
                var item = new Item();
                item.Data.Add(new Data() { Name = "stringProperty", Value = dataItem.StringProperty });
                item.Data.Add(new Data() { Name = "integerProperty", Value = dataItem.IntegerProperty.ToString() });
                doc.Collection.Items.Add(item);
            }
            return doc;
        }
    }

    public class TestInputReader : ICollectionJsonDocumentReader<TestInputOnly>
    {
        public TestInputOnly Read(WriteDocument document)
        {
            var testInputOnly = new TestInputOnly();
            testInputOnly.StringProperty = document.Template.Data.Single(d => d.Name == "stringProperty").Value;
            testInputOnly.IntegerProperty = int.Parse(document.Template.Data.Single(d => d.Name == "integerProperty").Value);
            
            return testInputOnly;
        }
    }

    public class TestInputAndOutputReaderAndWriter : ICollectionJsonDocumentReader<TestInputAndOutput>, ICollectionJsonDocumentWriter<TestInputAndOutput>
    {
        public TestInputAndOutput Read(WriteDocument document)
        {
            var testInputAndOutput = new TestInputAndOutput();
            testInputAndOutput.StringProperty = document.Template.Data.Single(d => d.Name == "stringProperty").Value;
            testInputAndOutput.IntegerProperty = int.Parse(document.Template.Data.Single(d => d.Name == "integerProperty").Value);

            return testInputAndOutput;
        }

        public ReadDocument Write(IEnumerable<TestInputAndOutput> data)
        {
            var doc = new ReadDocument();
            foreach (var dataItem in data)
            {
                var item = new Item();
                item.Data.Add(new Data() { Name = "stringProperty", Value = dataItem.StringProperty });
                item.Data.Add(new Data() { Name = "integerProperty", Value = dataItem.IntegerProperty.ToString() });
                doc.Collection.Items.Add(item);
            }
            return doc;
        }
    }

    [TestClass]
    public class TypeMappedFormatterTest
    {
        private TypeMappedCollectionJsonFormatter formatter = new TypeMappedCollectionJsonFormatter();
        private JObject _sampleWriteDocument;

        public TypeMappedFormatterTest()
        {
            formatter.RegisterWriter(new TestOutputWriter());
            formatter.RegisterReader(new TestInputReader());
            formatter.RegisterReader(new TestInputAndOutputReaderAndWriter());
            formatter.RegisterWriter(new TestInputAndOutputReaderAndWriter());

            _sampleWriteDocument = JObject.FromObject(new
            {
                template = new
                {
                    data = new object[]
                                {
                                    new {name = "stringProperty", value = "Hello World"},
                                    new {name = "integerProperty", value = "1337"}
                                }
                }
            });

        }

        [TestMethod]
        public void WhenWriterIsRegisteredShouldBeAbleToWriteSingleItem()
        {
            formatter.CanWriteType(typeof(TestOutputOnly)).ShouldBeTrue();
        }

        [TestMethod]
        public void WhenWriterIsRegisteredShouldBeAbleToWriteEnumerableOfItems()
        {
            formatter.CanWriteType(typeof(IEnumerable<TestOutputOnly>)).ShouldBeTrue();
        }

        [TestMethod]
        public void WhenWriterIsNotRegisteredShouldNotBeAbleToWriteSingleItem()
        {
            formatter.CanWriteType(typeof(TestInputOnly)).ShouldBeFalse();
        }

        [TestMethod]
        public void WhenWriterIsNotRegisteredShouldNotBeAbleToWriteEnumerableOfItems()
        {
            formatter.CanWriteType(typeof(IEnumerable<TestInputOnly>)).ShouldBeFalse();
        }

        [TestMethod]
        public void ShouldBeAbleToWriteReadDocument()
        {
            formatter.CanWriteType(typeof(ReadDocument));
        }

        [TestMethod]
        public void WhenReaderIsRegisteredShouldBeAbleToReadSingleItem()
        {
            formatter.CanReadType(typeof(TestInputOnly));
        }

        [TestMethod]
        public void WhenReaderIsNotRegisteredShouldNotBeAbleToReadSingleItem()
        {
            formatter.CanReadType(typeof(TestOutputOnly));
        }

        [TestMethod]
        public void ShouldBeAbleToReadWriteDocument()
        {
            formatter.CanReadType(typeof(WriteDocument));
        }

        [TestMethod]
        public void WhenReadingRegisteredTypeShouldReturnInstance()
        {
            var jsonDoc = _sampleWriteDocument.ToString();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(jsonDoc);
            writer.Flush();

            HttpContent content = new StringContent(String.Empty);
            HttpContentHeaders contentHeaders = content.Headers;
            contentHeaders.ContentLength = stream.Length;
            contentHeaders.ContentType = new MediaTypeHeaderValue("application/vnd.collection+json");

            stream.Seek(0, SeekOrigin.Begin);

            var obj = formatter.ReadFromStreamAsync(typeof(TestInputOnly), stream, content, null).Result;

            obj.ShouldNotBeNull();
            obj.ShouldBeType<TestInputOnly>();
            
            var testObj = (TestInputOnly)obj;
            testObj.StringProperty.ShouldEqual("Hello World");
            testObj.IntegerProperty.ShouldEqual(1337);
        }

        [TestMethod]
        public void WhenReadingWriteDocumentShouldReturnWriteDocument()
        {
            var jsonDoc = _sampleWriteDocument.ToString();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.Write(jsonDoc);
            writer.Flush();

            HttpContent content = new StringContent(String.Empty);
            HttpContentHeaders contentHeaders = content.Headers;
            contentHeaders.ContentLength = stream.Length;
            contentHeaders.ContentType = new MediaTypeHeaderValue("application/vnd.collection+json");

            stream.Seek(0, SeekOrigin.Begin);

            var newDoc = formatter.ReadFromStreamAsync(typeof(WriteDocument), stream, content, null).Result;//  as WriteDocument;

            newDoc.ShouldNotBeNull();
            newDoc.ShouldBeType<WriteDocument>();
        }

        [TestMethod]
        public void WhenWritingSingleRegisteredTypeShouldReturnJson()
        {
            var testObj = new TestOutputOnly() {StringProperty = "Hello World", IntegerProperty = 1337};

            var stream = new MemoryStream();

            formatter.WriteToStreamAsync(typeof(TestOutputOnly), testObj, stream, null, null).Wait();

            stream.Seek(0, SeekOrigin.Begin);

            var result = new StreamReader(stream).ReadToEnd();
            result.ShouldNotBeNull();

            var doc = JsonConvert.DeserializeObject<ReadDocument>(result);

            doc.Collection.ShouldNotBeNull();

            doc.Collection.Items.ShouldNotBeEmpty();
            doc.Collection.Items[0].Data.ShouldNotBeEmpty();
            doc.Collection.Items[0].Data[0].Name.ShouldEqual("stringProperty");
            doc.Collection.Items[0].Data[0].Value.ShouldEqual("Hello World");
            doc.Collection.Items[0].Data[1].Name.ShouldEqual("integerProperty");
            doc.Collection.Items[0].Data[1].Value.ShouldEqual("1337");
        }

        [TestMethod]
        public void WhenWritingMultipleRegisteredTypeShouldReturnJson()
        {
            var testObjs = new[]
                {
                    new TestOutputOnly() {StringProperty = "Hello World", IntegerProperty = 1337},
                    new TestOutputOnly() {StringProperty = "Hello Again", IntegerProperty = 4711}
                };

            var stream = new MemoryStream();

            formatter.WriteToStreamAsync(testObjs.GetType(), testObjs, stream, null, null).Wait();

            stream.Seek(0, SeekOrigin.Begin);

            var result = new StreamReader(stream).ReadToEnd();
            result.ShouldNotBeNull();

            var doc = JsonConvert.DeserializeObject<ReadDocument>(result);

            doc.Collection.ShouldNotBeNull();

            doc.Collection.Items.ShouldNotBeEmpty();
            doc.Collection.Items[0].Data.ShouldNotBeEmpty();
            doc.Collection.Items[0].Data[0].Name.ShouldEqual("stringProperty");
            doc.Collection.Items[0].Data[0].Value.ShouldEqual("Hello World");
            doc.Collection.Items[0].Data[1].Name.ShouldEqual("integerProperty");
            doc.Collection.Items[0].Data[1].Value.ShouldEqual("1337");
            doc.Collection.Items[1].Data[0].Name.ShouldEqual("stringProperty");
            doc.Collection.Items[1].Data[0].Value.ShouldEqual("Hello Again");
            doc.Collection.Items[1].Data[1].Name.ShouldEqual("integerProperty");
            doc.Collection.Items[1].Data[1].Value.ShouldEqual("4711");
        }
    }

    [TestClass]
    public class TypeMappedFormatterAttributeTest
    {
        public TypeMappedFormatterAttributeTest()
        {
            
        }

        [TestMethod]
        public void AttributeWithSingleInputReader()
        {
            var attr = new TypeMappedCollectionJsonFormatterAttribute(typeof(TestInputReader));

            var controllerSettings = new HttpControllerSettings(new HttpConfiguration());

            attr.Initialize(controllerSettings, null);

            var addedFormatter = controllerSettings.Formatters.SingleOrDefault(f => f is TypeMappedCollectionJsonFormatter);

            addedFormatter.ShouldNotBeNull();
            addedFormatter.CanReadType(typeof(TestInputOnly)).ShouldBeTrue();
            addedFormatter.CanReadType(typeof(TestOutputOnly)).ShouldBeFalse();
        }

        [TestMethod]
        public void AttributeWithMultipleReaders()
        {
            var attr = new TypeMappedCollectionJsonFormatterAttribute(typeof(TestInputReader), typeof(TestInputAndOutputReaderAndWriter));

            var controllerSettings = new HttpControllerSettings(new HttpConfiguration());

            attr.Initialize(controllerSettings, null);

            var addedFormatter = controllerSettings.Formatters.SingleOrDefault(f => f is TypeMappedCollectionJsonFormatter);

            addedFormatter.ShouldNotBeNull();
            addedFormatter.CanReadType(typeof(TestInputOnly)).ShouldBeTrue();
            addedFormatter.CanReadType(typeof(TestInputAndOutput)).ShouldBeTrue();
        }

        [TestMethod]
        public void AttributeWithSingleWriter()
        {
            var attr = new TypeMappedCollectionJsonFormatterAttribute(typeof(TestOutputWriter));

            var controllerSettings = new HttpControllerSettings(new HttpConfiguration());

            attr.Initialize(controllerSettings, null);

            var addedFormatter = controllerSettings.Formatters.SingleOrDefault(f => f is TypeMappedCollectionJsonFormatter);

            addedFormatter.ShouldNotBeNull();
            addedFormatter.CanWriteType(typeof(TestOutputOnly)).ShouldBeTrue();
            addedFormatter.CanWriteType(typeof(TestInputOnly)).ShouldBeFalse();
        }

        [TestMethod]
        public void AttributeWithMultipleWriters()
        {
            var attr = new TypeMappedCollectionJsonFormatterAttribute(typeof(TestOutputWriter), typeof(TestInputAndOutputReaderAndWriter));

            var controllerSettings = new HttpControllerSettings(new HttpConfiguration());

            attr.Initialize(controllerSettings, null);

            var addedFormatter = controllerSettings.Formatters.SingleOrDefault(f => f is TypeMappedCollectionJsonFormatter);

            addedFormatter.ShouldNotBeNull();
            addedFormatter.CanWriteType(typeof(TestOutputOnly)).ShouldBeTrue();
            addedFormatter.CanWriteType(typeof(TestInputAndOutput)).ShouldBeTrue();
        }
    }
}