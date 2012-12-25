using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Swagger.Net.Custom;
using Swagger.Net.Custom.Extensions;
using Swagger.Net._Test.Custom.Extensions;

namespace Swagger.Net._Test.Custom
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


}