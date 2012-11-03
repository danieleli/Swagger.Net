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
        private readonly IEnumerable<ApiDescription> _apiDescriptions;
        private readonly IApiDescriptionFactory _factory;
        private readonly XmlCommentDocumentationProvider _docProvider;

        public SwaggerActionFilter()
        {
            _factory = new ApiDescriptionFactory();
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetDocumentationProvider();
        }

        public SwaggerActionFilter(IEnumerable<ApiDescription> apiDescriptions, IDocumentationProvider docProvider, IApiDescriptionFactory factory)
        {
            _apiDescriptions = apiDescriptions;
            _factory = factory;
            _docProvider = (XmlCommentDocumentationProvider)docProvider;
        }

        // Intercept all request and handle swagger requests or pass through
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (IsDocRequest(actionContext)) return;

            var docs = GetDocs(actionContext);
            var formatter = actionContext.ControllerContext.Configuration.Formatters.JsonFormatter;

            actionContext.Response = WrapResponse(formatter, docs);
        }

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

        private Resource GetDocs(HttpActionContext actionContext)
        {
            var uri = actionContext.Request.RequestUri;
            var ctlrName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;

            var docs = _factory.CreateApiDescription(uri, ctlrName, _apiDescriptions);

            return docs;
        }
        
        private static HttpResponseMessage WrapResponse(JsonMediaTypeFormatter formatter, Resource docs)
        {
            var responseContent = new ObjectContent<Resource>(docs, formatter);
            var response = new HttpResponseMessage { Content = responseContent };
            return response;
        }

    }
}
