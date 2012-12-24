using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Xml.XPath;

namespace Swagger.Net.Custom.Extensions
{
    public static class ControllerExtensions
    {
        public static ControllerMetadata GetDocs(this HttpControllerDescriptor controller, IEnumerable<HttpActionDescriptor> actions, string parentControllerName, string relativePath, HttpMethod httpMethod)
        {
            return GetDocs(controller, actions, parentControllerName, relativePath,httpMethod, DocNavigator.Instance);
        }


        public static ControllerMetadata GetDocs(this HttpControllerDescriptor controller, IEnumerable<HttpActionDescriptor> actions, string parentControllerName, string relativePath, HttpMethod httpMethod, XPathNavigator docs)
        {
            return GetDocs(controller, actions,  parentControllerName, relativePath, httpMethod,
                           controller.ControllerType.XPathQuery(), docs);
        }


        public static ControllerMetadata GetDocs(this HttpControllerDescriptor controller, IEnumerable<HttpActionDescriptor> actions, string parentControllerName, string relativePath, HttpMethod httpMethod, string xPathQuery, XPathNavigator docs)
        {
            var controllerNode = docs.SelectSingleNode(xPathQuery);

            var controllerMeta = new ControllerMetadata
                {
                    Name = controller.ControllerName,
                    ParentController = parentControllerName,
                    Summary = Utils.GetNodeValue(controllerNode, "summary"),
                    Remarks = Utils.GetNodeValue(controllerNode, "remarks")
                };

            controllerMeta.ModelType = controller.ControllerType.GetDocs(docs);
            controllerMeta.Operations =
                actions.Select(a => a.GetDocs(parentControllerName, relativePath, httpMethod, docs));
            controllerMeta.Children = null;

            return controllerMeta;
        }
    }
}