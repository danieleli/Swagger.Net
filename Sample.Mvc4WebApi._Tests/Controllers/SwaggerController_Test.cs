using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Sample.Mvc4WebApi.App_Start;
using Swagger.Net.Factories;

namespace Swagger.Net._Test
{
    [TestClass]
    public class SwaggerController_Test
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            SwaggerNet.PreStart();
            SwaggerNet.ConfigureDocumentationProvider(@"C:\Users\danieleli\Documents\_projects\Swagger.Net\Sample.Mvc4WebApi\bin\Sample.Mvc4WebApi.xml", GlobalConfiguration.Configuration);
            var resourceFactory = new EndpointMetadataFactory("//app/virtual/path", GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions);
            var ctlr = new SwaggerController(resourceFactory);


            // Act
            var results = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;

            // Assert
            Debug.WriteLine(JsonConvert.SerializeObject(results));

        }
    }
}
