using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.XPath;
using Swagger.Net.Custom;

namespace Swagger.Net._Test.Custom
{
    public static class Extensions
    {
        const string TYPE_XPATH_QUERY = "/doc/members/member[@name='T:{0}']";

        public static string XPathQuery(this Type type)
        {
            return string.Format(TYPE_XPATH_QUERY, type.FullName);
        }

        public static TypeMetadata GetDocs(this Type type, XPathNavigator docs)
        {
            var node = docs.SelectSingleNode(type.XPathQuery());

            var rtn = new TypeMetadata()
            {
                Name = type.Name,
                Summary = GetNodeValue(node, "summary"),
                Remarks = GetNodeValue(node, "remarks"),
                Properties = GetPropertyDocs(type, docs)
            };

            return rtn;
        }

        private static List<PropertyMetadata> GetPropertyDocs(Type type, XPathNavigator docs)
        {
            var propertiesDocs = new List<PropertyMetadata>();
            foreach (var propertyInfo in type.GetProperties())
            {
                var pDoc = propertyInfo.GetDocs(docs);
                propertiesDocs.Add(pDoc);
            }
            return propertiesDocs;
        }

        private static string GetNodeValue(XPathNavigator node, string query)
        {
            if (node == null) return "N/A";

            var rtnNode = node.SelectSingleNode(query);
            return rtnNode == null ? "N/A" : rtnNode.Value.Trim();
        }
    }

    public static class PropertyExtensions
    {
        const string PROPERTY_XPATH_QUERY = "/doc/members/member[contains(@name,'P:{0}')]";
       
        public static string XPathQuery(this PropertyInfo propInfo)
        {
            var fullName = propInfo.ReflectedType.FullName + "." + propInfo.Name;
            return string.Format(PROPERTY_XPATH_QUERY,fullName);
        }

        public static PropertyMetadata GetDocs(this PropertyInfo propInfo, XPathNavigator docs)
        {
            var node = docs.SelectSingleNode(propInfo.XPathQuery());

            var rtn = new PropertyMetadata()
                {
                    Name = propInfo.Name,
                    DataType = propInfo.PropertyType.Name,
                    Remarks = GetNodeValue(node, "remarks"),
                    Summary = GetNodeValue(node, "summary")
                };

            return rtn;
        }

        private static string GetNodeValue(XPathNavigator node, string query)
        {
            if (node == null) return "N/A";

            var rtnNode = node.SelectSingleNode(query);
            return rtnNode == null ? "N/A" : rtnNode.Value.Trim();
        }
    }
}