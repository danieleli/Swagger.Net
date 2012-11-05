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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class SwaggerActionFilterAttribute : ActionFilterAttribute
    {
        #region --- fields & ctors ---
        
        private readonly ResourceMetadataFactory _factory;

        public SwaggerActionFilterAttribute()
        {
            _factory = new ResourceMetadataFactory();   
        }

        public SwaggerActionFilterAttribute(ResourceMetadataFactory factory)
        {
            _factory = factory;
        }

        #endregion // --- fields & ctors ---

        // Intercept all request and handle swagger requests or pass through
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (IsDocRequest(actionContext))
            {

                // Arrange
                var rootUrl = actionContext.Request.RequestUri.GetLeftPart(UriPartial.Authority);
                var ctlrName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;

                // Act
                var docs = _factory.GetDocs(rootUrl, ctlrName);

                // Answer
                var formatter = actionContext.ControllerContext.Configuration.Formatters.JsonFormatter;
                var response = WrapResponse(formatter, docs);

                actionContext.Response = response;
            }
            else
            {
                base.OnActionExecuting(actionContext);
            }
        }

        #region --- helpers ---

        private bool IsDocRequest(HttpActionContext actionContext)
        {
            var containsSwaggerKey = actionContext.ControllerContext.RouteData.Values.ContainsKey(G.SWAGGER);
            return containsSwaggerKey;
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
