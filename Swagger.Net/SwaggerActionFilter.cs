using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using Swagger.Net.Factories;
using Swagger.Net.Models;

namespace Swagger.Net
{
    /// <summary>
    /// Determines if any request hit the Swagger route. Moves on if not, otherwise responds with JSON Swagger spec doc
    /// </summary>
    public class SwaggerActionFilter : ActionFilterAttribute
    {
        #region --- fields & ctors ---
        
        private readonly IEnumerable<ApiDescription> _apiDescriptions;
        private readonly IResourceDescriptionFactory _factory;

        public SwaggerActionFilter()
        {
            _factory = new ResourceDescriptionFactory();
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
        }

        public SwaggerActionFilter(IEnumerable<ApiDescription> apiDescriptions, IResourceDescriptionFactory factory)
        {
            _apiDescriptions = apiDescriptions;
            _factory = factory;
        }

        #endregion // --- fields & ctors ---

        // Intercept all request and handle swagger requests or pass through
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (IsDocRequest(actionContext)) return;

            var docs = GetDocs(actionContext);
            var formatter = actionContext.ControllerContext.Configuration.Formatters.JsonFormatter;

            actionContext.Response = WrapResponse(formatter, docs);
        }

        #region --- helpers ---

        private bool IsDocRequest(HttpActionContext actionContext)
        {
            var isDocRequest = actionContext.ControllerContext.RouteData.Values.ContainsKey(SwaggerConstants.SWAGGER);

            if (!isDocRequest)
            {
                base.OnActionExecuting(actionContext);
                return true;
            }
            return false;
        }

        private ResourceDescription GetDocs(HttpActionContext actionContext)
        {
            var uri = actionContext.Request.RequestUri;
            var ctlrName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;

            var docs = _factory.CreateResourceDescription(uri, ctlrName);
            docs.apis = _factory.CreateApiElements(ctlrName, _apiDescriptions);

            // todo: models
            // docs.models = null;

            return docs;
        }

        private static HttpResponseMessage WrapResponse(JsonMediaTypeFormatter formatter, ResourceDescription docs)
        {
            var responseContent = new ObjectContent<ResourceDescription>(docs, formatter);
            var response = new HttpResponseMessage { Content = responseContent };
            return response;
        }

        #endregion // --- helpers ---
    }
}
