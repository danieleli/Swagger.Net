using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Swagger.Net.Custom;

namespace Swagger.Net._Test.Custom
{
    [TestClass]
    public class MetadataFactory_Test
    {
        MockRepository repo = new MockRepository();
        [TestMethod]
        public void NoControllers_Returns_EmptyMetadata()
        {
            var apis = new List<ApiDescription>();
            var factory = new MetadataFactory(apis, null);

            var meta = factory.CreateMetadata();

            Assert.AreEqual(0, meta.Count(), "metadata items");
        }


        [TestMethod]
        public void OneControllers_Returns_OneControllerMetadata()
        {

            
            var mockControllerDesc = repo.Stub<HttpControllerDescriptor>();
            mockControllerDesc.ControllerName = "Test";

            var mockAction = repo.Stub<HttpActionDescriptor>();
            mockAction.ControllerDescriptor = mockControllerDesc;
            mockAction.ReturnType.Stub(type => typeof (TestReturnType));

            var mockApi = repo.Stub<ApiDescription>();
            mockApi.ActionDescriptor = mockAction;
            

            var apis = new List<ApiDescription>() { mockApi };
            var factory = new MetadataFactory(apis, null);

            var meta = factory.CreateMetadata();

            Assert.AreEqual(1, meta.Count(), "metadata items");
        }
    }

    public class TestReturnType
    {
        
    }
}
