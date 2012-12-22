using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using Swagger.Net.Custom;

namespace Swagger.Net
{
    public static class SelectorFactory
    {
        const string METHOD_XPATH_QUERY = "/doc/members/member[@name='M:{0}']";
        
        const string ENUM_ITEM_XPATH_QUERY = "/doc/members/member[@name='F:{0}']";
        const string MEMBER_PARAM_XPATH_QUERY = "param[@name='{0}']";
        const string PROPERTY_XPATH_QUERY = "/doc/members/member[contains(@name,'P:{0}')]";

        
    }

    public class Temp
    {
        private static string GetNodeText(XPathNavigator node, string xpathQuery)
        {
            if (node != null)
            {
                var summaryNode = node.SelectSingleNode(xpathQuery);
                if (summaryNode != null)
                {
                    return summaryNode.Value.Trim();
                }
            }

            return "";// NO_DOCS_FOUND;
        }
    }




    public static class Extensions
    {
        const string TYPE_XPATH_QUERY = "/doc/members/member[@name='T:{0}']";
        public static string XPathQuery(this Type type)
        {
            return string.Format(TYPE_XPATH_QUERY, type.FullName);
        }

    }
}
