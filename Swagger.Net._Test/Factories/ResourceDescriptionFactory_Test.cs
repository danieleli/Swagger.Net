using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Rhino.Mocks;
using Swagger.Net.Factories;
using Swagger.Net.Models;

namespace Swagger.Net._Test.Factories
{
    [TestClass]
    public class ResourceDescriptionFactory_Test
    {
        private const string ROOT = "http://www.google.com";
        private const string VIRTUAL_DIR = "/the/vdir/of/app";
        private const string CONTROLLER_NAME = "myXXController";
        private const string RELATIVE_PATH = "SOME/RELATIVE/path";
        private const string DOCUMENTATION = "SOME DOCUmenation that si used";
        private const string ROUTE_TEMPLATE = "fjdkl/ffdklsa/{cc}/{id}";
        private readonly Uri _uri = new Uri(ROOT + "/this/is?field=3&test=mytest");

        private IResourceDescriptionFactory _factory;

        
        public void Setup()
        {
            var path = @"C:\Users\danieleli\Documents\_projects\Swagger.Net\Swagger.Net.WebApi\bin\Swagger.Net.WebApi.XML";
            var docProvider = new XmlCommentDocumentationProvider(path);
            _factory = new ResourceDescriptionFactory(VIRTUAL_DIR, docProvider);
            
        }

        [TestMethod]
        public void CreateResourceDesc_PopulatesRootDescProperties()
        {
            Setup();

            // Arrange
            ApiDescription apiDesc = TestHelper.GetApiDescription();
            var descriptions = new List<ApiDescription> { apiDesc };

            // Act
            var result = _factory.CreateResourceDescription(_uri, CONTROLLER_NAME);

            // Asset
            var expectedVersion = "1.2.3.4";

            Assert.AreEqual(expectedVersion, result.apiVersion, "api version");
            Assert.AreEqual(SwaggerConstants.SWAGGER_VERSION, result.swaggerVersion, "SwaggerVersion");
            Assert.AreEqual(ROOT + VIRTUAL_DIR, result.basePath, "BasePath");
            Assert.AreEqual(CONTROLLER_NAME, result.resourcePath, "resourcePath");
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
            var apis = _factory.CreateApiElements(CONTROLLER_NAME, descriptions);

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
            var apis = _factory.CreateApiElements(CONTROLLER_NAME, descriptions);

            // Asset
            Assert.AreEqual(0, apis.Count, "api count");

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
            var api = _factory.CreateApi(apiDesc);

            // Asset
            //Assert.AreEqual(0, apis.Count, "api count");

            Debug.WriteLine(JsonConvert.SerializeObject(api));
        }

        [TestMethod]
        public void CreateParameter_Returns()
        {
            Setup();
            // Arrange
            var config = GlobalConfiguration.Configuration;
            var path = @"C:\Users\danieleli\Documents\_projects\Swagger.Net\Swagger.Net.WebApi\bin\Swagger.Net.WebApi.XML";
            var docProvider = new XmlCommentDocumentationProvider(path);
            config.Services.Replace(typeof(IDocumentationProvider), docProvider);

            var apis = GlobalConfiguration.Configuration.Services.GetApiExplorer();
            

            foreach (var api in apis.ApiDescriptions)
            {
                
                foreach (var param in api.ParameterDescriptions)
                {
                    var rtnParam = _factory.CreateParameter(param);
                    Debug.WriteLine(JsonConvert.SerializeObject(rtnParam));
                }
            }
            

            // Act
            

            // Asset
            //Assert.AreEqual(0, apis.Count, "api count");

            
        }

        public static class TestHelper
        {
            public static ApiDescription GetApiDescription(string ctlrName = CONTROLLER_NAME, string docs = DOCUMENTATION, HttpMethod method = null)
            {
                var actionDesc = CreateActionDescriptor(ctrlName:ctlrName);
                method = method ?? HttpMethod.Get;
                var apiDesc = new ApiDescription
                {
                    RelativePath = RELATIVE_PATH,
                    Documentation = docs,
                    Route = new HttpRoute(ROUTE_TEMPLATE, new HttpRouteValueDictionary()),
                    HttpMethod = method,
                    ActionDescriptor = actionDesc
                };

                return apiDesc;
            }

            public static HttpActionDescriptor CreateActionDescriptor(string ctrlName, string paramName = "pname", bool isOptional = false, Type paramType = null)
            {
                paramType = paramType ?? typeof(string);
                var param = CreateParameter(paramName, paramType, isOptional);
                var parameters = new BindingList<HttpParameterDescriptor> { param };

                var actionDesc = MockRepository.GenerateStub<HttpActionDescriptor>();
                actionDesc.Stub(x => x.GetParameters()).Return(parameters);

                var ctlrDesc = MockRepository.GenerateStub<HttpControllerDescriptor>();
                ctlrDesc.ControllerName = ctrlName;

                actionDesc.ControllerDescriptor = ctlrDesc;
                return actionDesc;
            }

            public static HttpParameterDescriptor CreateParameter(string name, Type type, bool isOptional)
            {
                var p = MockRepository.GenerateStub<HttpParameterDescriptor>();
                p.Stub(x => x.ParameterName).Return(name);
                p.Stub(x => x.ParameterType).Return(type);
                p.Stub(x => x.IsOptional).Return(isOptional);

                return p;
            }
        }

  
    }
}