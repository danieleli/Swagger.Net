using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Controllers.Fakes;
using System.Web.Http.Description;
using System.Web.Http.Description.Fakes;
using System.Web.Http.Routing;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Swagger.Net.Factories;
using Swagger.Net.Models;

namespace Swagger.Net._Test.Factories
{
    [TestClass]
    public class ApiFactory_Test
    {
        const string ROOT = "http://www.google.com";
        const string VIRTUAL_DIR = "/the/vdir/of/app";
        const string CONTROLLER_NAME = "myXXController";
        private Uri _uri = new Uri(ROOT + "/this/is?field=3&test=mytest");
        const string RELATIVE_PATH = "SOME/RELATIVE/path";
        const string DOCUMENTATION = "SOME DOCUmenation that si used";

        [TestMethod]
        public void Test()
        {
            using (ShimsContext.Create())
            {
                var factory = new ApiFactory(VIRTUAL_DIR);
                var apiDesc = new ApiDescription()
                    {
                        RelativePath = RELATIVE_PATH,
                        Documentation = DOCUMENTATION,
                        Route = new HttpRoute("fjdkl/ffdklsa/{cc}/{id}", new HttpRouteValueDictionary()),
                        HttpMethod = HttpMethod.Get
                    };
                var parameter = new StubHttpParameterDescriptor();
                parameter.ParameterNameGet = () => "pname";
                parameter.ParameterTypeGet = () => typeof(string);
                parameter.IsOptionalGet = () => false;

                apiDesc.ActionDescriptor = new StubHttpActionDescriptor() { GetParameters01 = () => new BindingList<HttpParameterDescriptor>() { parameter } };
                
                apiDesc.ActionDescriptor.ControllerDescriptor =  new ShimHttpControllerDescriptor(){};
                apiDesc.ActionDescriptor.ControllerDescriptor.ControllerName = CONTROLLER_NAME;


                var descriptions = new List<ApiDescription>()
                                  {
                                      apiDesc
                                  };



                var result = factory.CreateResource(_uri, CONTROLLER_NAME, descriptions);

                Debug.WriteLine(JsonConvert.SerializeObject(result));
            }




        }


    }
}
