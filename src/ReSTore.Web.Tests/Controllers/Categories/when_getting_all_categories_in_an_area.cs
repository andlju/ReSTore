using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Web.Controllers;
using ReSTore.Web.Models;
using Should;
using WebApiContrib.Formatting.CollectionJson;

namespace ReSTore.Web.Tests.Controllers.Categories
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
            _document.Collection.Href.ToString().ShouldEqual("http://test.com/api/areas");
        }
    }

    [TestClass]
    public class when_getting_all_categories_in_an_area : given_some_test_data
    {
        private Category[] _categories;

        [TestInitialize]
        public void When()
        {
            var controller = new CategoriesController(Store);

            _categories = controller.Get(TestDataHelpers._Area2Id).ToArray();
        }

        [TestMethod]
        public void then_one_category_is_returned()
        {
            Assert.AreEqual(1, _categories.Length);
        }

        [TestMethod]
        public void then_category_id_is_correct()
        {
            Assert.AreEqual(TestDataHelpers._Category3Id, _categories[0].Id);
        }
    }
}