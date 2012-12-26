using System;
using System.Linq;
using System.Xml.XPath;

namespace Custom.ApiDescriber.Extensions
{
    public static class Utils
    {
        public static string GetNodeValue(XPathNavigator node, string query)
        {
            if (node == null) return "N/A";

            var rtnNode = node.SelectSingleNode(query);
            return rtnNode == null ? "N/A" : rtnNode.Value.Trim();
        }

        public static string GetCleanTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var rtn = type.Name.Substring(0, type.Name.Length - 2);
                
                var args =  type.GetGenericArguments().Select(t=>t.Name);
                rtn = rtn + "<" + string.Join(",", args) + ">";
                return rtn;
            }
            return type.Name;
        }
    }
}