using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Swagger.Net.Custom;
using Swagger.Net.Custom.Extensions;

namespace Swagger.Net._Test.Custom.Extensions
{
    [TestClass]
    public class ParameterExtensionsFixture
    {
        XPathNavigator docs = DocNavigator.Instance;

        [TestMethod]
        public void GetDocs()
        {

           // var stub = new  HttpParameterDescriptor;
           //// stub.ParameterNameGet = () =>  "test";

           // //var param = new ShimHttpParameterDescriptor(new StubHttpParameterDescriptor());
            
           // var paramMeta = stub.GetDocs(docs);

           // Debug.WriteLine("Name: " + paramMeta.Name);
           // Debug.WriteLine("Comment: " + paramMeta.Comment);
           // Debug.WriteLine("Type: " + paramMeta.Type);
        }
    }


}