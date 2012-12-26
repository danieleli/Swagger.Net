using Custom.ApiDescriber;
using Custom.ApiDescriber.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Swagger.Net._Test.Custom.Extensions
{
    [TestClass]
    public class TypeExtensionsFixture
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
        public void Returns_TypeMetadata_For_NullableInt()
        {
            var type = typeof(int?);
            var typeDocs = type.GetDocs();

            Assert.AreEqual(typeof(TypeMetadata), typeDocs.GetType(), "return type");
            Assert.AreEqual("Nullable<Int32>", typeDocs.Name, "Name");
        }
    }


}
