using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Web.Controllers;
using Should;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Tests.Controllers.Areas
{
    [TestClass]
    public class when_mapping_areas_to_a_ReadDocument
    {
        private ReadDocument _document;

        [TestInitialize]
        public void When()
        {
            var areas = TestDataHelpers.GetAreas();
            var mapper = new AreaHypermediaMapper();
            _document = mapper.Write(areas);
        }

        [TestMethod]
        public void then_document_version_is_1()
        {
            _document.Collection.Version.ShouldEqual("1.0");
        }

        [TestMethod]
        public void then_document_contains_three_items()
        {
            _document.Collection.Items.Count.ShouldEqual(3);
        }

        [TestMethod]
        public void then_item_links_are_correct()
        {
            _document.Collection.Href.ToString().ShouldEqual("/api/areas");
        }

        [TestMethod]
        public void then_item_contents_are_correct()
        {
            var item1 = _document.Collection.Items[1];

            item1.Data[0].Name.ShouldEqual("areaId");
            item1.Data[0].Value.ShouldEqual(TestDataHelpers._Area2Id.ToString());

            item1.Data[1].Name.ShouldEqual("name");
            item1.Data[1].Value.ShouldEqual("Area 2");
        }
    }
}