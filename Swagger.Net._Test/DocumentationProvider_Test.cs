using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Sample.Mvc4WebApi.Models;
using Swagger.Net._Test.Factories;

namespace Swagger.Net._Test
{
    [TestClass]
    public class DocumentationProvider_Test
    {
        private XmlCommentDocumentationProvider _docProvider;

        public void Setup()
        {
            _docProvider = new XmlCommentDocumentationProvider(TestHelper.XML_DOC_PATH);
        }
        [TestMethod]
        public void GetModels()
        {
            Setup();
            // Act
            var result = _docProvider.GetApiModel(typeof(BlogPost));

            Assert.IsNotNull(result, "Result is null");
            Debug.WriteLine(JsonConvert.SerializeObject(result));
        }
    }

    

}

namespace Sample.Mvc4WebApi.Models
{
    public class BlogPost
    {
    }

}
