using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Swagger.Net.Custom;
using Swagger.Net.Custom.Extensions;

namespace Swagger.Net._Test.Custom.Extensions
{
    [TestClass]
    public class ControllerExtensionsFixture
    {
        [TestMethod]
        public void Returns_Metadata_WithControllerName()
        {
            var actionData = GetControllerMeta();
            Assert.AreEqual(typeof(ActionMetadata), actionData.GetType(), "return type");
            Assert.AreEqual("GetFoo", actionData.Name, "Name");
        }

        [TestMethod]
        public void Returns_Metadata_With_ParentControllerName()
        {
            var actionData = GetControllerMeta();
            Assert.AreEqual(typeof(ActionMetadata), actionData.GetType(), "return type");
            Assert.AreEqual("Foo.GetFoo(int id) - Summary", actionData.Summary, "Summary");
            Assert.AreEqual("Foo.GetFoo(int id) - Remarks", actionData.Remarks, "Remarks");
        }

        [TestMethod]
        public void Returns_Metadata_With_ActionsMetadata()
        {

        }

        [TestMethod]
        public void Returns_With_ChildControllers()
        {
        }


        [TestMethod]
        public void Returns_ModelType()
        {
        }


        const string ACTION_NAME = "GetFoo";
        const string PARENT_CONTROLLER_NAME = "Customer";
        const string CONTROLLER_NAME = "CustomerOrder";
        const string RELATIVE_PATH = "CustomerOrder/{subid}";


        // Helper
        private ControllerMetadata GetControllerMeta()
        {
            
            var repo = new MockRepository();

            var returnType = typeof(Foo);
            var docs = DocNavigator.Instance;

            //var controllerMetadata = ControllerExtensions.GetDocs(PARENT_CONTROLLER_NAME);

            return null;

        }
    }
}