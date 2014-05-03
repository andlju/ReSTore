using System;
using System.Collections.Generic;
using WebApiContrib.CollectionJson;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Controllers.OrderControllers
{
    public class OrderCommandHypermediaMapper<TCmd> : 
        ICollectionJsonDocumentWriter<TCmd>,
        ICollectionJsonDocumentReader<TCmd> 
        where TCmd : new()
    {
        public TCmd Read(IWriteDocument document)
        {
            var command = document.Template.FromTemplate<TCmd>();
            
            return command;
        }

        public IReadDocument Write(IEnumerable<TCmd> data)
        {
            var doc = new ReadDocument();
            var collection = doc.Collection;
            collection.Version = "1.0";
            collection.Href = new Uri(string.Format("/api/commands/order/" + typeof(TCmd).Name.ToCase(Case.CamelCase)), UriKind.Relative);
            collection.Template.PopulateTemplate(typeof(TCmd));
            
            return doc;
        }

    }
}