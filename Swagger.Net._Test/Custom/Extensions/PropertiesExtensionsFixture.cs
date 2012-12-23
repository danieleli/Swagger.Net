using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swagger.Net.Custom.Extensions;

namespace Swagger.Net._Test.Custom.Extensions
{
    [TestClass]
    public class PropertiesExtensionsFixture
    {
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
}