using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Swagger.Net._Test
{
    [TestClass]
    public class UtilsFixture
    {
        [TestMethod]
        public void GetCleanTypeName_String()
        {
            var rtn = Utils.GetCleanTypeName(typeof (string));
            Assert.AreEqual("String", rtn, "Name");
        }

        [TestMethod]
        public void GetCleanTypeName_Foo()
        {
            var rtn = Utils.GetCleanTypeName(typeof(Foo));
            Assert.AreEqual("Foo", rtn, "Name");
        }


        [TestMethod]
        public void GetCleanTypeName_ListOfString()
        {
            var t = typeof (List<string>);
            var rtn = Utils.GetCleanTypeName(t);
            Assert.AreEqual("List<String>", rtn, "Name");
        }


        [TestMethod]
        public void GetCleanTypeName_IEnumOfInt()
        {
            var t = typeof(IEnumerable<int>);
            var rtn = Utils.GetCleanTypeName(t);
            Assert.AreEqual("IEnumerable<Int32>", rtn, "Name");
        }
    }

    public class Foo
    {
    }
}