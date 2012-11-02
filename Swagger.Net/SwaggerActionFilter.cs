using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
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
        private readonly XmlCommentDocumentationProvider _docProvider;

        public SwaggerActionFilter()
        {
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetDocumentationProvider();
        }

        public SwaggerActionFilter(IEnumerable<ApiDescription> apiDescriptions, IDocumentationProvider docProvider)
        {
            _apiDescriptions = apiDescriptions;
            _docProvider = (XmlCommentDocumentationProvider)docProvider;
        }

        /// <summary>
        /// Executes each request to give either a JSON Swagger spec doc or passes through the request
        /// </summary>
        /// <param name="actionContext">Context of the action</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var isDocRequest = actionContext.ControllerContext.RouteData.Values.ContainsKey(SwaggerConstants.SWAGGER);

            if (!isDocRequest)
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            var uri = actionContext.Request.RequestUri;
            var ctlrName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            
            var docs = getDocs(uri, ctlrName);
            var formatter = actionContext.ControllerContext.Configuration.Formatters.JsonFormatter;
            
            var responseContent = new ObjectContent<Resource>(docs, formatter);
            var response = new HttpResponseMessage { Content = responseContent };

            actionContext.Response = response;
        }

        private Resource getDocs(Uri uri, string ctlrName)
        {
            var factory = new ApiFactory();

            var descs = _apiDescriptions.Where(
                    d => d.ActionDescriptor.ControllerDescriptor.ControllerName == ctlrName);

            var rtn = factory.CreateResource(uri, ctlrName, descs);

            return rtn;
        }
    }
}
