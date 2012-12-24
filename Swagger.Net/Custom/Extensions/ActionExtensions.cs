using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Xml.XPath;

namespace Swagger.Net.Custom.Extensions
{
    public static class ActionExtensions
    {
        public static ActionMetadata GetDocs(this HttpActionDescriptor action,
                                                        string parentControllerName,
                                                        string relativePath)
        {
            return GetDocs(action, parentControllerName, relativePath, DocNavigator.Instance);
        }


        public static ActionMetadata GetDocs(this HttpActionDescriptor action,
                                                        string parentControllerName,
                                                        string relativePath,
                                                        XPathNavigator docs)
        {

            var rtn = GetDocs(action.ActionName, action.ReturnType, parentControllerName,
                           relativePath, action.SupportedHttpMethods, action.XPathQuery(), docs);
            rtn.Params = null; // GetParams(action);

            return rtn;
        }


        // No dependency on HttpActionDescriptor - Easier Testing
        public static ActionMetadata GetDocs(string actionName, Type returnType, string parentControllerName, string relativePath, IEnumerable<HttpMethod> httpMethod, string xPathQuery, XPathNavigator docs)
        {
            var node = docs.SelectSingleNode(xPathQuery);



            var path = relativePath.ToLower();
            var altPath = "";
            if (parentControllerName != null)
            {
                altPath = path;
                path = "";// GetAlternatePath(parentControllerName,action.ControllerDescriptor.ControllerName,relativePath).ToLower();
            }

            var op = new ActionMetadata()
            {
                Name = actionName,
                HttpMethod = httpMethod.ToString(),
                RelativePath = path,
                AlternatePath = altPath,
                Summary = Utils.GetNodeValue(node, "summary"),
                Remarks = Utils.GetNodeValue(node, "remarks"),
                ReturnType = returnType.GetDocs(docs),
                ReturnsComment = Utils.GetNodeValue(node, "returns"),
                ErrorResponses = null
            };
            return op;
        }
    }
}