using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using WebApiContrib.CollectionJson;
using WebApiContrib.Formatting.CollectionJson.Client;

namespace WebApiContrib.Formatting.CollectionJson
{
    public class TypeMappedCollectionJsonFormatterAttribute : Attribute, IControllerConfiguration
    {
        private readonly Type[] _mappers;

        public TypeMappedCollectionJsonFormatterAttribute(params Type[] mappers)
        {
            _mappers = mappers;
        }

        public void Initialize(HttpControllerSettings settings,
            HttpControllerDescriptor descriptor)
        {
            var formatter = new TypeMappedCollectionJsonFormatter();
            foreach (var mapper in _mappers)
            {
                var mapperInterfaces = mapper.GetInterfaces();
                foreach (var mapperInterface in mapperInterfaces)
                {
                    if (mapperInterface.Name == "ICollectionJsonDocumentWriter`1")
                    {
                        var type = mapperInterface.GetGenericArguments()[0];
                        formatter.GetType()
                                 .GetMethod("RegisterWriter")
                                 .MakeGenericMethod(type)
                                 .Invoke(formatter, new object[] { Activator.CreateInstance(mapper) });
                    }
                    if (mapperInterface.Name == "ICollectionJsonDocumentReader`1")
                    {
                        var type = mapperInterface.GetGenericArguments()[0];
                        formatter.GetType()
                                 .GetMethod("RegisterReader")
                                 .MakeGenericMethod(type)
                                 .Invoke(formatter, new object[] { Activator.CreateInstance(mapper) });
                    }
                }
            }

            settings.Formatters.Add(formatter);
        }
    }

    public class TypeMappedCollectionJsonFormatter : CollectionJsonFormatter
    {
        private IDictionary<Type, object> _readers = new Dictionary<Type, object>();
        private IDictionary<Type, object> _writers = new Dictionary<Type, object>();

        public void RegisterReader<TModel>(ICollectionJsonDocumentReader<TModel> reader)
        {
            _readers.Add(typeof(TModel), reader);
        }

        public void RegisterWriter<TModel>(ICollectionJsonDocumentWriter<TModel> writer)
        {
            _writers.Add(typeof(TModel), writer);
        }

        public override bool CanWriteType(Type type)
        {
            if (base.CanWriteType(type))
                return true;

            if (type.IsGenericType && type.GetInterface("IEnumerable") != null)
            {
                type = type.GetGenericArguments()[0];
            }
            return _writers.ContainsKey(type);
        }

        public override bool CanReadType(Type type)
        {
            if (base.CanReadType(type))
                return true;

            return _readers.ContainsKey(type);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (type == typeof(ReadDocument) || type == typeof(WriteDocument))
                return base.ReadFromStreamAsync(typeof(WriteDocument), readStream, content, formatterLogger);

            return base.ReadFromStreamAsync(typeof(WriteDocument), readStream, content, formatterLogger).
                ContinueWith(state =>
                                 {
                                     var doc = state.Result as WriteDocument;

                                     return InvokeReadMethod(_readers[type], doc);
                                 });

        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
                                                TransportContext transportContext)
        {
            if (type == typeof(WriteDocument) || type == typeof(ReadDocument))
            {
                return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
            }

            var enumInterface = type.Name == "IEnumerable`1" ? type : type.GetInterface("IEnumerable`1");

            if (enumInterface != null)
            {
                type = enumInterface.GetGenericArguments()[0];
            }
            else
            {
                var tmpArray = Array.CreateInstance(type, 1);
                tmpArray.SetValue(value, 0);
                value = tmpArray;
            }

            var readDoc = InvokeWriteMethod(_writers[type], value);
            return base.WriteToStreamAsync(typeof(ReadDocument), readDoc, writeStream, content, transportContext);
        }

        private object InvokeReadMethod(object reader, WriteDocument writeDocument)
        {
            return reader.GetType().GetMethod("Read").Invoke(reader, new object[] { writeDocument });
        }

        private ReadDocument InvokeWriteMethod(object writer, object model)
        {
            return writer.GetType().GetMethod("Write").Invoke(writer, new object[] { model }) as ReadDocument;
        }
    }
}