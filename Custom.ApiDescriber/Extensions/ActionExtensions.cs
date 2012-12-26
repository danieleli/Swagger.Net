using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Xml.XPath;

namespace Custom.ApiDescriber.Extensions
{
    public static class ActionExtensions
    {
        public static ActionMetadata GetDocs(this HttpActionDescriptor action, string parentControllerName, string relativePath, HttpMethod httpMethod)
        {
            return GetDocs(action, parentControllerName, relativePath, httpMethod, DocNavigator.Instance);
        }


        public static ActionMetadata GetDocs(this HttpActionDescriptor action, string parentControllerName, string relativePath, HttpMethod httpMethod, XPathNavigator docs)
        {
            return GetDocs(action.ControllerDescriptor.ControllerName, action.ActionName, action.ReturnType, parentControllerName, 
                            relativePath, httpMethod, action.XPathQuery(), docs, action.GetParameters());
        }


        public static ActionMetadata GetDocs(string controllerName, string actionName, Type returnType, string parentControllerName, string relativePath,
                                                HttpMethod httpMethod, string xPathQuery, XPathNavigator docs, IEnumerable<HttpParameterDescriptor> paramz)
        {
            var actionNode = docs.SelectSingleNode(xPathQuery);

            var actionMeta = GetDocs(controllerName, actionName, returnType, parentControllerName, relativePath, httpMethod, actionNode);
            actionMeta.Params = paramz.Select(p => p.GetDocs(actionNode)).ToList();

            // todo: ErrorResponses
            actionMeta.ErrorResponses = null;

            return actionMeta;
        }

        // No dependency on HttpActionDescriptor - Easier Testing
        public static ActionMetadata GetDocs(string controllerName, string actionName, Type returnType, string parentControllerName, string relativePath, HttpMethod httpMethod, XPathNavigator actionNode)
        {
            var alternatePath = GetAlternatePath(controllerName, parentControllerName, relativePath).ToLower();


            var returnTypeMeta = returnType==null ? null : returnType.GetDocs();

            var actionMeta = new ActionMetadata()
            {
                Name = actionName,
                HttpMethod = httpMethod.ToString(),
                RelativePath = relativePath.ToLower(),
                AlternatePath = alternatePath,
                Summary = Utils.GetNodeValue(actionNode, "summary"),
                Remarks = Utils.GetNodeValue(actionNode, "remarks"),
                ReturnType = returnTypeMeta,
                ReturnsComment = Utils.GetNodeValue(actionNode, "returns"),
                ErrorResponses = null
            };
            return actionMeta;
        }


        public static string GetAlternatePath(string currentControllerName, string parentControllerName, string relativePath)
        {
            parentControllerName = parentControllerName ?? currentControllerName;

            var rtn = "";

            if (currentControllerName != parentControllerName && relativePath.Contains(currentControllerName))
            {
                var shortName = currentControllerName.Substring(parentControllerName.Length);
                rtn += parentControllerName + "/{id}/" + shortName;


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

                rtn += morePath;
            }
            return rtn;
        }


    }
}