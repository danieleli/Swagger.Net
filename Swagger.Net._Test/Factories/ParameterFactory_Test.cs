using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Sample.Mvc4WebApi.Models;
using Swagger.Net.Factories;

namespace Swagger.Net._Test.Factories
{
    [TestClass]
    public class ParameterFactory_Test
    {
        private ParameterFactory _factory;

        public void Setup()
        {
            var docProvider = new XmlCommentDocumentationProvider(TestHelper.XML_DOC_PATH);
            _factory = new ParameterFactory(docProvider);

        }
        [TestMethod]
        public void GetParameterType_With_IEnumerable()
        {
            Setup();
            var type = typeof(IEnumerable<Int32>);

            var rtn = ModelFactory.GetDataType(type).Name;
            
            Assert.AreEqual("Int32", rtn, "returned value");
        }

        [TestMethod]
        public void GetParameterType_With_Array()
        {
            Setup();
            var type = typeof(Int32[]);

            var rtn = ModelFactory.GetDataType(type).Name;

            Debug.WriteLine(rtn);
            Assert.AreEqual("Int32", rtn, "returned value");
        }

        [TestMethod]
        public void GetParameterType_With_ListT()
        {
            Setup();
            var type = typeof(List<Int32>);

            var rtn = ModelFactory.GetDataType(type).Name;

            Debug.WriteLine(rtn);
            Assert.AreEqual("Int32", rtn, "returned value");
        }

        [TestMethod]
        public void CreateParameter_ReturnsWith_DataType_IsOptional_ParamName_ParamType()
        {
            // Arrange
            Setup();
            var paramSource = ApiParameterSource.FromUri;
            var dataType = typeof(BlogPost);
            var isOptional = true;
            var paramName = "param3";

            var input = TestHelper.CreateParameter(paramName, dataType, isOptional, paramSource);


            // Act
            dynamic rtnParam = _factory.CreateParameter(input, "");


            // Assert
            Assert.AreEqual(dataType.Name, rtnParam.dataType, "param source (body, uri, unknown");
            Assert.AreEqual(!isOptional, rtnParam.required, "is required");
            Assert.AreEqual(paramName, rtnParam.name, "param name");
            Assert.AreEqual(G.QUERY, rtnParam.paramType, "param Type");
            Debug.WriteLine(JsonConvert.SerializeObject((object)rtnParam));
        }

        [TestMethod]
        public void CreateParameter_Sets_AllowableValues()
        {
            // Arrange
            Setup();
            var paramSource = ApiParameterSource.FromUri;
            var dataType = typeof(BlogPost);
            var isOptional = true;
            var paramName = "param3";

            var input = TestHelper.CreateParameter(paramName, dataType, isOptional, paramSource);


            // Act
            dynamic rtnParam = _factory.CreateParameter(input, "");

            var expected = new object();
            // Assert
            Assert.AreEqual(expected, rtnParam.allowableValues, "Allowable values");
            Debug.WriteLine(JsonConvert.SerializeObject((object)rtnParam));
        }

        [TestMethod]
        public void CreateParameter_Returns_FromUri_When_ParamName_IsInRoute()
        {
            // Arrange
            Setup();
            var paramSource = ApiParameterSource.FromUri;
            var dataType = typeof(BlogPost);
            var isOptional = true;
            var paramName = "myparam";

            var input = TestHelper.CreateParameter(paramName, dataType, isOptional, paramSource);


            // Act
            dynamic rtnParam = _factory.CreateParameter(input, TestHelper.ROUTE_TEMPLATE);

            // Assert
            Assert.AreEqual(G.PATH, rtnParam.paramType, "param Type");
            Debug.WriteLine(JsonConvert.SerializeObject((object)rtnParam));
        }

        [TestMethod]
        public void CreateParameter_Returns_FromBody_When_ParamSouce_IsFromBody()
        {
            // Arrange
            Setup();
            var paramSource = ApiParameterSource.FromBody;
            var dataType = typeof(BlogPost);
            var isOptional = true;
            var paramName = "myparam";

            var input = TestHelper.CreateParameter(paramName, dataType, isOptional, paramSource);


            // Act
            dynamic rtnParam = _factory.CreateParameter(input, TestHelper.ROUTE_TEMPLATE);

            // Assert
            Assert.AreEqual(G.BODY, rtnParam.paramType, "param Type");
            Debug.WriteLine(JsonConvert.SerializeObject((object)rtnParam));
        }

        [TestMethod]
        public void CreateParameter_Returns_FromBody_When_ParamSouce_IsUnknown()
        {
            // Arrange
            Setup();
            var paramSource = ApiParameterSource.Unknown;
            var dataType = typeof(BlogPost);
            var isOptional = true;
            var paramName = "myparam";

            var input = TestHelper.CreateParameter(paramName, dataType, isOptional, paramSource);


            // Act
            dynamic rtnParam = _factory.CreateParameter(input, TestHelper.ROUTE_TEMPLATE);

            // Assert
            Assert.AreEqual(G.BODY, rtnParam.paramType, "param Type");
            Debug.WriteLine(JsonConvert.SerializeObject((object)rtnParam));
        }

        [TestMethod]
        public void CreateParameter_Sets_AllowableMultiple()
        {
            // Arrange
            Setup();
            var paramSource = ApiParameterSource.FromUri;
            var dataType = typeof(BlogPost);
            var isOptional = true;
            var paramName = "param3";

            var input = TestHelper.CreateParameter(paramName, dataType, isOptional, paramSource);


            // Act
            dynamic rtnParam = _factory.CreateParameter(input, "");

            // Assert
            Assert.AreEqual(false, rtnParam.allowMultiple, "Allowable values");
            Debug.WriteLine(JsonConvert.SerializeObject((object)rtnParam));
        }

        [TestMethod]
        public void IEnumerable_AllowMutiple_True()
        {
            // Arrange
            Setup();
            var paramSource = ApiParameterSource.FromUri;
            var dataType = typeof(IEnumerable<Int32>);
            var isOptional = true;
            var paramName = "param3";

            var input = TestHelper.CreateParameter(paramName, dataType, isOptional, paramSource);

            // Act
            dynamic rtnParam = _factory.CreateParameter(input,"fjdskf/fjdksla");

            // Assert
            Assert.AreEqual(true, rtnParam.allowMultiple, "Allowable values");
            Debug.WriteLine(JsonConvert.SerializeObject((object)rtnParam));
        }
    }
}
