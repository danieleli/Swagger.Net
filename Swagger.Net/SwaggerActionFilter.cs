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
        private readonly IApiExplorer _apiExplorer;
        private readonly ISwaggerFactory _swaggerFactory;
        private readonly XmlCommentDocumentationProvider _docProvider;

        public SwaggerActionFilter()
        {
            _apiExplorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();
            _swaggerFactory = new SwaggerFactory();
            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetDocumentationProvider();
        }

        public SwaggerActionFilter(IApiExplorer apiExplorer, IDocumentationProvider docProvider, ISwaggerFactory swaggerFactory)
        {
            _apiExplorer = apiExplorer;
            _docProvider = (XmlCommentDocumentationProvider)docProvider;
            _swaggerFactory = swaggerFactory;
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

            var response = new HttpResponseMessage();

            response.Content = new ObjectContent<Resource>(
                getDocs(actionContext),
                actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);

            actionContext.Response = response;
        }

        private Resource getDocs(HttpActionContext actionContext)
        {

            var factory = new ApiFactory();
            var ctx = actionContext.ControllerContext;
            
            var rtn = factory.CreateResource(ctx.Request.RequestUri, ctx.ControllerDescriptor.ControllerName,
                                   _apiExplorer.ApiDescriptions);
            
            
            return rtn;
        }
    }
}
