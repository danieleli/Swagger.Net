using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using Custom.ApiDescriber;
using Custom.ApiDescriber.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

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

        [TestMethod]
        public void Returns_ReturnsComment()
        {
            var actionData = GetActionMeta();
            Assert.AreEqual("returns a foo by id", actionData.ReturnsComment, "action returns comment");
        }


        [TestMethod]
        public void Returns_RelativePath()
        {
            var actionData = GetActionMeta();
            Assert.AreEqual(RELATIVE_PATH.ToLower(), actionData.RelativePath, "relative path");
        }

        [TestMethod]
        public void Returns_AlternatePath()
        {
            var actionData = GetActionMeta();
            Assert.AreEqual("/customer/{id}/order/{subid}", actionData.AlternatePath, "alternate path");
        }

        [TestMethod]
        public void Returns_HttpMethods()
        {
            var actionData = GetActionMeta();
            Assert.AreEqual("GET", actionData.HttpMethod, "http method");
        }


        [TestMethod]
        public void GetAlternatePath()
        {
            Assert.Fail("not implemented");
        }
        
        [TestMethod]
        public void GetParamsMeta()
        {
            var actionData = GetActionMeta();
            Assert.AreEqual(1, actionData.Params.Count(), "Param count");
        }


        const string ACTION_NAME = "GetFoo";
        const string PARENT_CONTROLLER_NAME = "Customer";
        const string CONTROLLER_NAME = "CustomerOrder";
        const string RELATIVE_PATH = "CustomerOrder/{subid}";


        // Helper
        private ActionMetadata GetActionMeta()
        {
            var xPathQuery = string.Format(XPathQueries.METHOD,
                                           "Swagger.Net._Test.Custom.Extensions.Foo.GetFoo(System.Int32)");
            MockRepository repo = new MockRepository();

            var returnType = typeof(Foo);
            var docs = DocNavigator.Instance;
            var supportedMethod = HttpMethod.Get;
            var paramz = new List<HttpParameterDescriptor>();

            var paramDescriptor = GetParam(repo);

            paramz.Add(paramDescriptor);

            var actionMetadata = ActionExtensions.GetDocs(CONTROLLER_NAME, ACTION_NAME, returnType, PARENT_CONTROLLER_NAME, RELATIVE_PATH, supportedMethod, xPathQuery, docs, paramz);

            return actionMetadata;

        }

        private static HttpParameterDescriptor GetParam(MockRepository repo)
        {
            var paramDescriptor = repo.PartialMock<HttpParameterDescriptor>(null);
            paramDescriptor.Stub(descriptor => descriptor.ParameterName).Return("id").Repeat.Any();
            paramDescriptor.Stub(descriptor => descriptor.ParameterType).Return(typeof (int)).Repeat.Any();

            paramDescriptor.Replay();
            return paramDescriptor;
        }
    }
}