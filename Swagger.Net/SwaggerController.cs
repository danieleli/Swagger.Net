using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Swagger.Net
{
    public class SwaggerController : ApiController
    {
        /// <summary>
        /// Get the resource description of the api for swagger documentation
        /// </summary>
        /// <remarks>It is very convenient to have this information available for generating clients. This is the entry point for the swagger UI
        /// </remarks>
        /// <returns>JSON document representing structure of API</returns>
        public HttpResponseMessage Get()
        {
            var docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetDocumentationProvider();

            ResourceListing r = SwaggerFactory.CreateResourceListing(ControllerContext);
            List<string> uniqueControllers = new List<string>();

            foreach (var api in GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions)
            {
                string controllerName = api.ActionDescriptor.ControllerDescriptor.ControllerName;
                if (uniqueControllers.Contains(controllerName) ||
                      controllerName.ToUpper().Equals(SwaggerFactory.SWAGGER.ToUpper())) continue;

                uniqueControllers.Add(controllerName);

                ResourceApi rApi = SwaggerFactory.CreateResourceApi(api);
                r.apis.Add(rApi);
            }

            HttpResponseMessage resp = new HttpResponseMessage();

            resp.Content = new ObjectContent<ResourceListing>(r, ControllerContext.Configuration.Formatters.JsonFormatter);            
            
            return resp;
        }
    }
}
