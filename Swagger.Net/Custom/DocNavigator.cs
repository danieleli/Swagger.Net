using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            _navigator = new XPathDocument(@"..\Debug\Swagger.Net._Test.XML").CreateNavigator();
        }
    }
}
