using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Swagger.Net.Factories;

namespace Swagger.Net._Test
{
    [TestClass]
    public class EndpointDescriptionFactory_Test
    {

        const string ROOT = "http://www.google.com";
        const string VIRTUAL_DIR = "/the/vdir/of/app";
        const string CONTROLLER_NAME = "myXXController";

        [TestMethod]
        public void GetResourceListing_NoApis()
        {
            var uri = new Uri(ROOT + "/this/is?field=3&test=mytest");
            var apiDescs = new List<ApiDescription>();

            var factory = new EndpointDescriptionFactory(VIRTUAL_DIR, apiDescs);

            var listing = factory.CreateResourceListing(uri);

            Assert.AreEqual(0, listing.apis.Count, "api count");
            Assert.AreEqual("1.2.3.4", listing.apiVersion, "api version");
            Assert.AreEqual(ROOT+VIRTUAL_DIR, listing.basePath, "basePath");
            Assert.AreEqual(null, listing.resourcePath, "resourcePath");
            Assert.AreEqual("2.0", listing.swaggerVersion, "swaggerVersion");

            Debug.WriteLine(JsonConvert.SerializeObject(listing));
            
        }

        [TestMethod]
        public void GetResourceListing_OneApis()
        {
            
            var uri = new Uri(ROOT + "/this/is?field=3&test=mytest");
         
            var apiDescs = new List<ApiDescription>(){ 
                new ApiDescription()
                    {
                        ActionDescriptor = new ReflectedHttpActionDescriptor(){ControllerDescriptor = new HttpControllerDescriptor(){ControllerName = "somecontrolerName"}},
                        RelativePath = "relativePathHere",
                        Documentation = "somedocs"
                    },
            };
            var factory = new EndpointDescriptionFactory(VIRTUAL_DIR, apiDescs);

            var listing = factory.CreateResourceListing(uri);

            Assert.AreEqual(1, listing.apis.Count, "api count");
            Assert.AreEqual("1.2.3.4", listing.apiVersion, "api version");
            Assert.AreEqual(ROOT + VIRTUAL_DIR, listing.basePath, "basePath");
            Assert.AreEqual(null, listing.resourcePath, "resourcePath");
            Assert.AreEqual("2.0", listing.swaggerVersion, "swaggerVersion");

            Debug.WriteLine(JsonConvert.SerializeObject(listing));

        }
    }

}
