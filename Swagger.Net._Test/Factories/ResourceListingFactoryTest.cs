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
    public class ResourceListingFactoryTest
    {
        [TestMethod]
        public void GetResourceListing_NoApis()
        {
            var root = "http://www.google.com";
            var virtualDir = "/the/vdir/of/app";
            var factory = new ResourceListingFactory(virtualDir);
            var uri = new Uri(root+"/this/is?field=3&test=mytest");
            var controllerName = "myXXController";

            var apiDescs = new List<ApiDescription>();

            var listing = factory.CreateResourceListing(uri, controllerName, apiDescs);

            Assert.AreEqual(0, listing.apis.Count, "api count");
            Assert.AreEqual("1.2.3.4", listing.apiVersion, "api version");
            Assert.AreEqual(root+virtualDir, listing.basePath, "basePath");
            Assert.AreEqual(controllerName, listing.resourcePath, "resourcePath");
            Assert.AreEqual("2.0", listing.swaggerVersion, "swaggerVersion");

            Debug.WriteLine(JsonConvert.SerializeObject(listing));
            
        }

        [TestMethod]
        public void GetResourceListing_OneApis()
        {
            var root = "http://www.google.com";
            var virtualDir = "/the/vdir/of/app";
            var factory = new ResourceListingFactory(virtualDir);
            var uri = new Uri(root + "/this/is?field=3&test=mytest");
            var controllerName = "myXXController";

            var apiDescs = new List<ApiDescription>(){ 
                new ApiDescription()
                    {
                        ActionDescriptor = new ReflectedHttpActionDescriptor(){ControllerDescriptor = new HttpControllerDescriptor(){ControllerName = "somecontrolerName"}},
                        RelativePath = "relativePathHere",
                        Documentation = "somedocs"
                    },
            };

            var listing = factory.CreateResourceListing(uri, controllerName, apiDescs);

            Assert.AreEqual(1, listing.apis.Count, "api count");
            Assert.AreEqual("1.2.3.4", listing.apiVersion, "api version");
            Assert.AreEqual(root + virtualDir, listing.basePath, "basePath");
            Assert.AreEqual(controllerName, listing.resourcePath, "resourcePath");
            Assert.AreEqual("2.0", listing.swaggerVersion, "swaggerVersion");

            Debug.WriteLine(JsonConvert.SerializeObject(listing));

        }
    }

}
