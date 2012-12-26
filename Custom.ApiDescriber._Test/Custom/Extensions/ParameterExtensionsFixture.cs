using System.Web.Http.Controllers;
using System.Xml.XPath;
using Custom.ApiDescriber;
using Custom.ApiDescriber.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Swagger.Net._Test.Custom.Extensions
{
    [TestClass]
    public class ParameterExtensionsFixture
    {
        XPathNavigator docs = DocNavigator.Instance;
        MockRepository repo = new MockRepository();

        [TestMethod]
        public void GetDocs()
        {

            var xpath = string.Format(XPathQueries.METHOD,  "Swagger.Net._Test.Custom.Extensions.Foo.GetFoo(System.Int32)");
            var actionDocs = docs.SelectSingleNode(xpath);

            var paramDescriptor = repo.PartialMock<HttpParameterDescriptor>(null);
            paramDescriptor.Stub(descriptor => descriptor.ParameterName).Return("id").Repeat.Any();
            paramDescriptor.Stub(descriptor => descriptor.ParameterType).Return(typeof(int)).Repeat.Any();

            paramDescriptor.Replay();
            var paramMeta = paramDescriptor.GetDocs(actionDocs);

            Assert.AreEqual("id", paramMeta.Name, "Name Property");
            Assert.AreEqual("id param comment", paramMeta.Comment, "Comment Property");
            Assert.AreEqual("Int32", paramMeta.Type, "Type");
        }
    }


}