using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Web.Controllers;
using ReSTore.Web.Models;

namespace ReSTore.Web.Tests.Controllers.Categories
{
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
            Assert.AreEqual(TestDataHelpers._Category3Id, _categories[0].CategoryId);
        }
    }
}