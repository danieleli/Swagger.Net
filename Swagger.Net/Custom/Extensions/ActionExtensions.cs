using System;
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
                                                        string relativePath, 
                                                        HttpMethod httpMethod)
        {
            return GetDocs(action, parentControllerName, relativePath, httpMethod, DocNavigator.Instance);
        }


        public static ActionMetadata GetDocs(this HttpActionDescriptor action, 
                                                        string parentControllerName, 
                                                        string relativePath, 
                                                        HttpMethod httpMethod, 
                                                        XPathNavigator docs)
        {
            var node = docs.SelectSingleNode(action.XPathQuery());

            //var paramz =  GetParams(action);
            var returnType = action.ReturnType.GetDocs();

            var path = relativePath.ToLower();
            var altPath = "";
            if (parentControllerName != null)
            {
                altPath = path;
                path = "";// GetAlternatePath(parentControllerName,action.ControllerDescriptor.ControllerName,relativePath).ToLower();
            }

            var op = new ActionMetadata()
            {
                Name = action.ActionName,
                HttpMethod = httpMethod.ToString(),
                RelativePath = path,
                AlternatePath = altPath,
                Summary = Utils.GetNodeValue(node, "summary"),
                Remarks = Utils.GetNodeValue(node, "remarks"),
                ReturnType = returnType,
                ReturnsComment = "",//_docProvider.GetResponseClass(action),
                Params = null, //paramz,
                ErrorResponses = null
            };
            return op;
        }


        public static ActionMetadata GetDocs(string xPathQuery, 
            Type returnType,
            string actionName,
                                                 string parentControllerName,
                                                 string relativePath,
                                                 HttpMethod httpMethod,
                                                 XPathNavigator docs)
        {
            var node = docs.SelectSingleNode(xPathQuery);

            //var paramz =  GetParams(action);

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
                ReturnsComment = "",//_docProvider.GetResponseClass(action),
                Params = null, //paramz,
                ErrorResponses = null
            };
            return op;
        }
    }
}