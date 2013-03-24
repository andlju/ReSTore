using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReSTore.Web.Controllers;
using ReSTore.Web.Models;

namespace ReSTore.Web.Tests.Controllers.Areas
{
    [TestClass]
    public class when_getting_all_areas : given_some_test_data
    {
        private Area[] _areas;
        
        [TestInitialize]
        public void When()
        {
            var controller = new AreasController(Store);

            _areas = controller.Get().ToArray();
        }

        [TestMethod]
        public void then_three_items_are_returned()
        {
            Assert.AreEqual(3, _areas.Length);
        }

        [TestMethod]
        public void then_names_of_items_are_correct()
        {
            Assert.AreEqual("Area 1", _areas[0].Name);
        }

    }
}