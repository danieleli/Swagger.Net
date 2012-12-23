using System.Xml.XPath;

namespace Swagger.Net.Custom.Extensions
{
    public static class Utils
    {
        public static string GetNodeValue(XPathNavigator node, string query)
        {
            if (node == null) return "N/A";

            var rtnNode = node.SelectSingleNode(query);
            return rtnNode == null ? "N/A" : rtnNode.Value.Trim();
        }
    }
}