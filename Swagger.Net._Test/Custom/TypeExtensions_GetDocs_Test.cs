using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swagger.Net;
using Swagger.Net.Custom;
using Swagger.Net._Test.Custom.Extensions;
using Swagger.Net._Test.Factories;

namespace Swagger.Net._Test.Custom
{
    [TestClass]
    public class TypeExtensions_GetDocs_Test
    {
  

        [TestMethod]
        public void Returns_TypeMetadataWithName()
        {
            var type = typeof(object);
            var typeDocs = type.GetDocs();

            Assert.AreEqual(typeof(TypeMetadata), typeDocs.GetType(), "return type");
            Assert.AreEqual("Object", typeDocs.Name, "Name");
        }

        [TestMethod]
        public void MissingSummary_Returns_NA()
        {
            var type = typeof(object);
            var typeDocs = type.GetDocs();
            Assert.AreEqual("N/A", typeDocs.Summary, "Summary");
        }



        [TestMethod]
        public void Returns_Summary()
        {
            var type = typeof(Foo);
            var typeDocs = type.GetDocs();

            Assert.AreEqual("Foo Summary", typeDocs.Summary, "Summary");
        }

        [TestMethod]
        public void Returns_Remarks()
        {
            var type = typeof(Foo);
            var typeDocs = type.GetDocs();

            Assert.AreEqual("Foo Remarks", typeDocs.Remarks, "Remarks");
        }

        [TestMethod]
        public void Returns_Properties()
        {
            var type = typeof(Foo);
            var typeDocs = type.GetDocs();

            Assert.IsNotNull(typeDocs.Properties, "Docs.Properties");
            Assert.AreNotEqual(0, typeDocs.Properties.Count(), "Property Count");
        }


        [TestMethod]
        public void Returns_Property_Name()
        {
            var type = typeof(Foo);
            var typeDocs = type.GetDocs();

            var fooNameDocs = typeDocs.Properties.Single(p => p.Name == "FooName");

            Assert.IsNotNull(fooNameDocs, "FooName property not found.");
        }



        [TestMethod]
        public void Returns_Property_DataType()
        {
            var type = typeof(Foo);
            var typeDocs = type.GetDocs();

            var fooNameDocs = typeDocs.Properties.Single(p => p.Name == "FooName");

            Assert.AreEqual(fooNameDocs.DataType, "String", "Property Data Type");
        }

        [TestMethod]
        public void Returns_Property_Summary()
        {
            var type = typeof(Foo);
            var typeDocs = type.GetDocs();

            var fooNameDocs = typeDocs.Properties.Single(p => p.Name == "FooName");

            Assert.AreEqual(fooNameDocs.Summary, "Foo.FooName Summary");
            
        }

        [TestMethod]
        public void Returns_Property_Remarks()
        {
            var type = typeof(Foo);
            var typeDocs = type.GetDocs();

            var fooNameDocs = typeDocs.Properties.Single(p => p.Name == "FooName");

            Assert.AreEqual(fooNameDocs.Remarks, "Foo.FooName Remarks");

        }

    }

    /// <summary>
    /// Foo Summary
    /// </summary>
    /// <remarks>Foo Remarks</remarks>
    public class Foo
    {
        /// <summary>
        /// Foo.FooName Summary
        /// </summary>
        /// <remarks>Foo.FooName Remarks</remarks>
        public string FooName { get; set; }
        
    }
}
