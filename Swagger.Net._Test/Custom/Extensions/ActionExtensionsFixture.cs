using System.Diagnostics;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swagger.Net.Custom;
using Swagger.Net.Custom.Extensions;

namespace Swagger.Net._Test.Custom.Extensions
{
    [TestClass]
    public class ActionExtensionsFixture
    {
        [TestMethod]
        public void Returns_Metadata_WithName()
        {
            var actionData = GetActionMeta();
            Assert.AreEqual(typeof(ActionMetadata), actionData.GetType(), "return type");
            Assert.AreEqual("GetFoo", actionData.Name, "Name");
        }

        [TestMethod]
        public void Returns_Metadata_With_SummaryAndRemarks()
        {
            var actionData = GetActionMeta();
            Assert.AreEqual(typeof(ActionMetadata), actionData.GetType(), "return type");
            Assert.AreEqual("Foo.GetFoo(int id) - Summary", actionData.Summary, "Summary");
            Assert.AreEqual("Foo.GetFoo(int id) - Remarks", actionData.Remarks, "Remarks");
        }

        [TestMethod]
        public void Returns_Metadata_With_ReturnType_Metadata()
        {
            var actionData = GetActionMeta();
            Assert.AreEqual(typeof(TypeMetadata), actionData.ReturnType.GetType(), "return type type");

        }


        // Helper
        private ActionMetadata GetActionMeta()
        {
            var xPathQuery = string.Format(XPathQueries.METHOD,
                                           "Swagger.Net._Test.Custom.Extensions.Foo.GetFoo(System.Int32)");
            var returnType = typeof(Foo);
            var actionName = "GetFoo";
            var parentControllerName = "";
            var relativePath = "";
            var docs = DocNavigator.Instance;
            var supportedMethods = new[] { HttpMethod.Get, HttpMethod.Post };

            var actionMetadata = ActionExtensions.GetDocs(actionName, returnType,
                                                          parentControllerName, relativePath, supportedMethods, xPathQuery, docs);

            return actionMetadata;

        }

    }
}