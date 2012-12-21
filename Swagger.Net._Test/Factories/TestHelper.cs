using System;
using System.ComponentModel;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using Rhino.Mocks;

namespace Swagger.Net._Test.Factories
{
    public static class TestHelper
    {
        public const string ROOT = "http://www.google.com";
        public const string VIRTUAL_DIR = "/the/vdir/of/app";
        public const string CONTROLLER_NAME = "myXXController";
        public const string RELATIVE_PATH = "SOME/RELATIVE/path";
        public const string DOCUMENTATION = "SOME DOCUmenation that si used";
        public const string ROUTE_TEMPLATE = "fjdkl/ffdklsa/{myparam}/{id}";
        public const string XML_DOC_PATH = @"..\..\..\Sample.WebApi\bin\Sample.WebApi.XML";

        public static ApiDescription GetApiDescription(string ctlrName = CONTROLLER_NAME, string docs = DOCUMENTATION, HttpMethod method = null)
        {
            var actionDesc = CreateActionDescriptor(ctrlName: ctlrName);
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
            var param = CreateParameter(paramName, paramType, isOptional, ApiParameterSource.FromBody);
            var parameters = new BindingList<HttpParameterDescriptor> { param.ParameterDescriptor };

            var actionDesc = MockRepository.GenerateStub<HttpActionDescriptor>();
            actionDesc.Stub(x => x.GetParameters()).Return(parameters);

            var ctlrDesc = MockRepository.GenerateStub<HttpControllerDescriptor>();
            ctlrDesc.ControllerName = ctrlName;

            actionDesc.ControllerDescriptor = ctlrDesc;
            return actionDesc;
        }

        public static ApiParameterDescription CreateParameter(string name, Type type, bool isOptional, ApiParameterSource source)
        {
            var p = MockRepository.GenerateStub<HttpParameterDescriptor>();
            p.Stub(x => x.ParameterName).Return(name);
            p.Stub(x => x.ParameterType).Return(type);
            p.Stub(x => x.IsOptional).Return(isOptional);

            var apiParam = new ApiParameterDescription() { ParameterDescriptor = p, Name = name, Source = source, Documentation = "yada" };
            return apiParam;
        }
    }
}