using System.Configuration;
using System.Xml.XPath;

namespace Custom.ApiDescriber
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
