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
        public void Returns_ActionMetadataWithName()
        {

            var xPathQuery = string.Format(XPathQueries.METHOD, "Swagger.Net._Test.Custom.Extensions.Foo.GetFoo(System.Int32)");
            var returnType = typeof (Foo);
            var actionName = "GetFoo";
            var parentControllerName = "";
            var relativePath = "";
            var docs = DocNavigator.Instance;

            var actionMetadata = ActionExtensions.GetDocs(xPathQuery, returnType, actionName, parentControllerName,
                                                          relativePath, HttpMethod.Get, docs);

            Assert.AreEqual(typeof(ActionMetadata), actionMetadata.GetType(), "return type");
            Assert.AreEqual(actionName, actionMetadata.Name, "Name");
            Debug.WriteLine(actionMetadata);
        }

    }
}