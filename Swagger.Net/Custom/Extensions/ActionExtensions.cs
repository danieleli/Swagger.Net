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
                                                        string relativePath,
                                                        HttpMethod httpMethod)
        {
            return GetDocs(action, parentControllerName, relativePath,  httpMethod, DocNavigator.Instance);
        }


        public static ActionMetadata GetDocs(this HttpActionDescriptor action,
                                                        string parentControllerName,
                                                        string relativePath,
                                                        HttpMethod httpMethod,
                                                        XPathNavigator docs)
        {
        
            var rtn = GetDocs(action.ControllerDescriptor.ControllerName, action.ActionName, action.ReturnType, parentControllerName, relativePath, httpMethod, action.XPathQuery(), docs);
            rtn.Params = GetParams(action.GetParameters(), docs);
            return rtn;
        }

        private static IEnumerable<ParamMetadata> GetParams(IEnumerable<HttpParameterDescriptor> param, XPathNavigator docs)
        {
            var rtn = param.Select(p => p.GetDocs(docs));
            return rtn;
        }


        // No dependency on HttpActionDescriptor - Easier Testing
        public static ActionMetadata GetDocs(string controllerName, string actionName, Type returnType, string parentControllerName, string relativePath, HttpMethod httpMethod, string xPathQuery, XPathNavigator docs)
        {
            var node = docs.SelectSingleNode(xPathQuery);
            var alternatePath = GetAlternatePath(controllerName, parentControllerName, relativePath).ToLower();
           
   
            var op = new ActionMetadata()
            {
                Name = actionName,
                HttpMethod = httpMethod.ToString(),
                RelativePath = relativePath.ToLower(),
                AlternatePath = alternatePath,
                Summary = Utils.GetNodeValue(node, "summary"),
                Remarks = Utils.GetNodeValue(node, "remarks"),
                ReturnType = returnType.GetDocs(docs),
                ReturnsComment = Utils.GetNodeValue(node, "returns"),
                ErrorResponses = null
            };
            return op;
        }


        public static string GetAlternatePath(string currentControllerName, string parentControllerName, string relativePath)
        {
            var path = "/" + relativePath;

            if (currentControllerName != parentControllerName && relativePath.Contains(currentControllerName))
            {
                var shortName = currentControllerName.Substring(parentControllerName.Length);
                path = "/" + parentControllerName + "/{id}/" + shortName;


                var morePath = relativePath.Substring(currentControllerName.Length);
                if (morePath.Contains("/{id}") && morePath.ToLower().Contains("subid={subid}"))
                {
                    morePath = morePath.Replace("/{id}", "/{subId}");
                    morePath = morePath.ToLower().Replace("subid={subid}", "");
                }
                else
                {
                    morePath = morePath.Replace("/{id}", "");
                }

                if (morePath.IndexOf("?") > 0 && morePath.IndexOf("?") == morePath.Length - 1)
                {
                    morePath = morePath.Substring(0, morePath.Length - 1);
                }

                path += morePath;
            }
            return path;
        }


    }
}