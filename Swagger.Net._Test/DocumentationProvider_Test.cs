using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swagger.Net.WebApi.Models;

namespace Swagger.Net._Test
{
    [TestClass]
    public class DocumentationProvider_Test
    {
        [TestMethod]
        public void GetModels()
        {
            // Arrange
            var path = @"C:\Users\danieleli\Documents\_projects\Swagger.Net\Swagger.Net.WebApi\bin\Swagger.Net.WebApi.XML";
            var docProvider = new XmlCommentDocumentationProvider(path);

            // Act
            var result = docProvider.GetApiModel(typeof(BlogPost));

            Assert.IsNotNull(result, "Result is null");

        }
    }
}
