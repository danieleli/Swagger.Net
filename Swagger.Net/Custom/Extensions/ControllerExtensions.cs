using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Xml.XPath;

namespace Swagger.Net.Custom.Extensions
{
    public static class ControllerExtensions
    {
        public static ControllerMetadata GetDocs(this HttpControllerDescriptor controller, string parentControllerName)
        {
            return GetDocs(controller, parentControllerName, DocNavigator.Instance);
        }


        public static ControllerMetadata GetDocs(this HttpControllerDescriptor controller, string parentControllerName, XPathNavigator docs)
        {
            return GetDocs(controller, parentControllerName, controller.ControllerType.XPathQuery(), docs);
        }


        public static ControllerMetadata GetDocs(this HttpControllerDescriptor controller, string parentControllerName, string xPathQuery, XPathNavigator docs)
        {
            var controllerNode = docs.SelectSingleNode(xPathQuery);

            var controllerMeta = new ControllerMetadata
                {
                    Name = controller.ControllerName,
                    ParentController = parentControllerName,
                    Summary = Utils.GetNodeValue(controllerNode, "summary"),
                    Remarks = Utils.GetNodeValue(controllerNode, "remarks")
                };

            return controllerMeta;
        }

    }
}