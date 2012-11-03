using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
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
    public class ApiDescriptionFactory_Test
    {
        private const string ROOT = "http://www.google.com";
        private const string VIRTUAL_DIR = "/the/vdir/of/app";
        private const string CONTROLLER_NAME = "myXXController";
        private const string RELATIVE_PATH = "SOME/RELATIVE/path";
        private const string DOCUMENTATION = "SOME DOCUmenation that si used";
        private const string ROUTE_TEMPLATE = "fjdkl/ffdklsa/{cc}/{id}";
        private readonly Uri _uri = new Uri(ROOT + "/this/is?field=3&test=mytest");

        [TestMethod]
        public void Test()
        {
            
                // Arrange
                ApiDescription apiDesc = GetApiDescription();
                var descriptions = new List<ApiDescription> {apiDesc};
                var factory = new ApiDescriptionFactory(VIRTUAL_DIR);

                // Act
                Resource result = factory.CreateApiDescription(_uri, CONTROLLER_NAME, descriptions);

                // Asset (visually)
                Debug.WriteLine(JsonConvert.SerializeObject(result));
            
        }

        private static ApiDescription GetApiDescription(string docs = DOCUMENTATION, HttpMethod method = null)
        {
            var actionDesc = CreateActionDescriptor();
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

        private static HttpActionDescriptor CreateActionDescriptor(string ctrlName = CONTROLLER_NAME, string paramName = "pname", bool isOptional = false, Type paramType = null)
        {
            paramType = paramType ?? typeof (string);
            var param = CreateParameter(paramName, paramType, isOptional);
            var parameters = new BindingList<HttpParameterDescriptor> {param};

            var actionDesc = MockRepository.GenerateStub<HttpActionDescriptor>();
            actionDesc.Stub(x => x.GetParameters()).Return(parameters);

            var ctlrDesc = MockRepository.GenerateStub<HttpControllerDescriptor>();
            ctlrDesc.ControllerName= ctrlName;

            actionDesc.ControllerDescriptor= ctlrDesc;
            return actionDesc;
        }

        private static HttpParameterDescriptor CreateParameter(string name, Type type, bool isOptional)
        {
            var p = MockRepository.GenerateStub<HttpParameterDescriptor>();
            p.Stub(x => x.ParameterName).Return(name);
            p.Stub(x => x.ParameterType).Return(type);
            p.Stub(x => x.IsOptional).Return(isOptional);
            
            return p;
        }
    }
}