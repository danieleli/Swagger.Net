using System.Reflection;
using System.Xml.XPath;

namespace Swagger.Net.Custom.Extensions
{
    public static class PropertyExtensions
    {

        public static string XPathQuery(this PropertyInfo propInfo)
        {
            var fullName = propInfo.ReflectedType.FullName + "." + propInfo.Name;
            return string.Format(XPathQueries.PROPERTY, fullName);
        }

        public static PropertyMetadata GetDocs(this PropertyInfo propInfo, XPathNavigator docs)
        {
            var node = docs.SelectSingleNode(propInfo.XPathQuery());

            var rtn = new PropertyMetadata
                {
                    Name = propInfo.Name,
                    DataType = propInfo.PropertyType.Name,
                    Remarks = Utils.GetNodeValue(node, "remarks"),
                    Summary = Utils.GetNodeValue(node, "summary")
                };

            return rtn;
        }

        
    }
}