using System.Reflection;
using System.Xml.XPath;

namespace Swagger.Net.Custom.Extensions
{
    public static class PropertyExtensions
    {

        public static PropertyMetadata GetDocs(this PropertyInfo propInfo, XPathNavigator docs)
        {
            var node = docs.SelectSingleNode(propInfo.XPathQuery());

            var rtn = new PropertyMetadata
                {
                    Name = propInfo.Name,
                    DataType = Utils.GetCleanTypeName(propInfo.PropertyType),
                    Remarks = Utils.GetNodeValue(node, "remarks"),
                    Summary = Utils.GetNodeValue(node, "summary")
                };

            return rtn;
        }

        
    }
}