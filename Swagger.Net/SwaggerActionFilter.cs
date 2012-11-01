using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Swagger.Net
{
    /// <summary>
    /// Determines if any request hit the Swagger route. Moves on if not, otherwise responds with JSON Swagger spec doc
    /// </summary>
    public class SwaggerActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Executes each request to give either a JSON Swagger spec doc or passes through the request
        /// </summary>
        /// <param name="actionContext">Context of the action</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var docRequest = actionContext.ControllerContext.RouteData.Values.ContainsKey(SwaggerFactory.SWAGGER);

            if (!docRequest)
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            HttpResponseMessage response = new HttpResponseMessage();

            response.Content = new ObjectContent<ResourceListing>(
                getDocs(actionContext),
                actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);

            actionContext.Response = response;
        }

        private ResourceListing getDocs(HttpActionContext actionContext)
        {
            var docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetDocumentationProvider();

            ResourceListing r = SwaggerFactory.CreateResourceListing(actionContext);

            foreach (var api in GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions)
            {
                string apiControllerName = api.ActionDescriptor.ControllerDescriptor.ControllerName;
                if (api.Route.Defaults.ContainsKey(SwaggerFactory.SWAGGER) ||
                    apiControllerName.ToUpper().Equals(SwaggerFactory.SWAGGER.ToUpper())) 
                    continue;

                // Make sure we only report the current controller docs
                if (!apiControllerName.Equals(actionContext.ControllerContext.ControllerDescriptor.ControllerName))
                    continue;

                ResourceApi rApi = SwaggerFactory.CreateResourceApi(api);
                r.apis.Add(rApi);

                ResourceApiOperation rApiOperation = SwaggerFactory.CreateResourceApiOperation(api, docProvider);
                rApi.operations.Add(rApiOperation);

                foreach (var param in api.ParameterDescriptions)
                {
                    ResourceApiOperationParameter parameter = SwaggerFactory.CreateResourceApiOperationParameter(api, param, docProvider);
                    rApiOperation.parameters.Add(parameter);
                }
            }
            
            return r;
        }
    }
}
