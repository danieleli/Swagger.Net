using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.XPath;

namespace Swagger.Net.Custom
{
    public sealed class DocNavigator
    {
        private readonly XPathNavigator _navigator;

        static readonly DocNavigator _instance = new DocNavigator();

        public static XPathNavigator Instance
        {
            get { return _instance._navigator; }
        }
                           
        private DocNavigator()
        {
            var filename = ConfigurationManager.AppSettings["code_comments_file_name"];
            filename = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.TrimEnd("Swagger.Net.DLL".ToCharArray()) + filename;
            _navigator = new XPathDocument(filename).CreateNavigator();
        }
    }
}
