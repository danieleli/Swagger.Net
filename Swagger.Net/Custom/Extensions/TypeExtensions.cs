using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace Swagger.Net.Custom.Extensions
{
    public static class TypeExtensions
    {

        public static string XPathQuery(this Type type)
        {
            return string.Format(XPathQueries.TYPE, type.FullName);
        }

        public static TypeMetadata GetDocs(this Type type)
        {
            return GetDocs(type, DocNavigator.Instance);
        }

        public static TypeMetadata GetDocs(this Type type, XPathNavigator docs)
        {
            var node = docs.SelectSingleNode(type.XPathQuery());

            var rtn = new TypeMetadata
                {
                Name = type.Name,
                Summary = Utils.GetNodeValue(node, "summary"),
                Remarks = Utils.GetNodeValue(node, "remarks"),
                Properties = GetPropertyDocs(type, docs)
            };

            return rtn;
        }

        private static IEnumerable<PropertyMetadata> GetPropertyDocs(Type type, XPathNavigator docs)
        {
            return type.GetProperties().Select(propertyInfo => propertyInfo.GetDocs(docs));
        }
    }
}