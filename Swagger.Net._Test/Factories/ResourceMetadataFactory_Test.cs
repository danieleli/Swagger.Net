using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Swagger.Net.Factories;
using Swagger.Net.WebApi.Models;

namespace Swagger.Net._Test.Factories
{
    // todo: parameter: allowableValues and allowMultiple
    // todo: errorResponse
    // todo: models

    [TestClass]
    public class ResourceMetadataFactory_Test
    {
        private readonly Uri _uri = new Uri(TestHelper.ROOT + "/this/is?field=3&test=mytest");

        private ResourceMetadataFactory _factory;


        public void Setup()
        {
            var path = @"C:\Users\danieleli\Documents\_projects\Swagger.Net\Swagger.Net.WebApi\bin\Swagger.Net.WebApi.XML";
            var docProvider = new XmlCommentDocumentationProvider(path);
            _factory = new ResourceMetadataFactory(TestHelper.VIRTUAL_DIR, docProvider, new ParameterMetadataFactory());

        }

        [TestMethod]
        public void CreateResourceDesc_PopulatesRootDescProperties()
        {
            Setup();

            // Arrange
            ApiDescription apiDesc = TestHelper.GetApiDescription();
            var descriptions = new List<ApiDescription> { apiDesc };

            // Act
            var result = _factory.CreateResourceMetadata(TestHelper.ROOT, TestHelper.CONTROLLER_NAME);

            // Asset
            var expectedVersion = "1.2.3.4";

            Assert.AreEqual(expectedVersion, result.apiVersion, "api version");
            Assert.AreEqual(G.SWAGGER_VERSION, result.swaggerVersion, "SwaggerVersion");
            Assert.AreEqual(TestHelper.ROOT + TestHelper.VIRTUAL_DIR, result.basePath, "BasePath");
            Assert.AreEqual(TestHelper.CONTROLLER_NAME, result.resourcePath, "resourcePath");
            Assert.AreEqual(0, result.apis.Count, "Api count");
            Assert.AreEqual(0, result.models.Count, "model count");

            Debug.WriteLine(JsonConvert.SerializeObject(result));
        }

        [TestMethod]
        public void CreateApiElements_ReturnsApis()
        {
            Setup();
            // Arrange
            ApiDescription apiDesc = TestHelper.GetApiDescription();
            var descriptions = new List<ApiDescription> { apiDesc };

            // Act
            var apis = _factory.CreateApiElements(descriptions);

            // Asset
            Assert.AreEqual(1, apis.Count, "api count");

            Debug.WriteLine(JsonConvert.SerializeObject(apis));
        }

        [TestMethod]
        public void CreateApiElements_WithNoMatchingApiDescriptions_ReturnsNoApis()
        {
            Setup();
            // Arrange
            ApiDescription apiDesc = TestHelper.GetApiDescription("anotherCtlr");
            var descriptions = new List<ApiDescription> { apiDesc };

            // Act
            var apis = _factory.CreateApiElements(descriptions);

            // Asset
            Assert.AreEqual(1, apis.Count, "api count");

            Debug.WriteLine(JsonConvert.SerializeObject(apis));
        }

        [TestMethod]
        public void CreateApi_ReturnsApi()
        {
            Setup();
            // Arrange
            ApiDescription apiDesc = TestHelper.GetApiDescription();
            var descriptions = new List<ApiDescription> { apiDesc };

            // Act
            var api = _factory.CreateApiRoot(apiDesc);

            // Asset
            //Assert.AreEqual(0, apis.Count, "api count");

            Debug.WriteLine(JsonConvert.SerializeObject(api));
        }

    }
}