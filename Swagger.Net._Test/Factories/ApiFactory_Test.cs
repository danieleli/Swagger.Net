using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Controllers.Fakes;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Swagger.Net.Factories;

namespace Swagger.Net._Test.Factories
{
    [TestClass]
    public class ApiFactory_Test
    {
        const string ROOT = "http://www.google.com";
        const string VIRTUAL_DIR = "/the/vdir/of/app";
        const string CONTROLLER_NAME = "myXXController";
        const string RELATIVE_PATH = "SOME/RELATIVE/path";
        const string DOCUMENTATION = "SOME DOCUmenation that si used";
        private Uri _uri = new Uri(ROOT + "/this/is?field=3&test=mytest");

        [TestMethod]
        public void Test()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var apiDesc = GetApiDescription();
                var descriptions = new List<ApiDescription>(){ apiDesc };
                var factory = new ApiFactory(VIRTUAL_DIR);

                // Act
                var result = factory.CreateResource(_uri, CONTROLLER_NAME, descriptions);

                // Asset (visually)
                Debug.WriteLine(JsonConvert.SerializeObject(result));
            }




        }

        private static ApiDescription GetApiDescription()
        {
            var apiDesc = new ApiDescription()
                              {
                                  RelativePath = RELATIVE_PATH,
                                  Documentation = DOCUMENTATION,
                                  Route = new HttpRoute("fjdkl/ffdklsa/{cc}/{id}", new HttpRouteValueDictionary()),
                                  HttpMethod = HttpMethod.Get
                              };

            var parameter = CreateParameter("pname", typeof (string), false);

            apiDesc.ActionDescriptor = new StubHttpActionDescriptor()
                                           {GetParameters01 = () => new BindingList<HttpParameterDescriptor>() {parameter}};
            apiDesc.ActionDescriptor.ControllerDescriptor = new ShimHttpControllerDescriptor() {};
            apiDesc.ActionDescriptor.ControllerDescriptor.ControllerName = CONTROLLER_NAME;
            return apiDesc;
        }

        private static StubHttpParameterDescriptor CreateParameter(string name, Type type, bool isOptional)
        {
            var parameter = new StubHttpParameterDescriptor
                                {
                                    ParameterNameGet = () => name,
                                    ParameterTypeGet = () => type,
                                    IsOptionalGet = () => isOptional
                                };
            return parameter;
        }
    }
}
