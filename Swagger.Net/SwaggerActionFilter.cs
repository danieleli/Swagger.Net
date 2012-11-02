using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;
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
            var docRequest = actionContext.ControllerContext.RouteData.Values.ContainsKey(SwaggerConstants.SWAGGER);

            if (!docRequest)
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            var response = new HttpResponseMessage();

            response.Content = new ObjectContent<ResourceListing>(
                getDocs(actionContext),
                actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);

            actionContext.Response = response;
        }

        private ResourceListing getDocs(HttpActionContext actionContext)
        {
            var r = _swaggerFactory.CreateResourceListing(actionContext);
            var apiModels = new Dictionary<string, Model>();

            foreach (var api in _apiExplorer.ApiDescriptions)
            {
                var apiControllerName = api.ActionDescriptor.ControllerDescriptor.ControllerName;
                if (api.Route.Defaults.ContainsKey(SwaggerConstants.SWAGGER) ||
                    apiControllerName.ToUpper().Equals(SwaggerConstants.SWAGGER.ToUpper())) 
                    continue;

                // Make sure we only report the current controller docs
                if (!apiControllerName.Equals(actionContext.ControllerContext.ControllerDescriptor.ControllerName))
                    continue;

                var rApi = _swaggerFactory.CreateResourceApi(api);
               // r.apis.Add(rApi);

                var rApiOperation = _swaggerFactory.CreateApiOperation(api, _docProvider);
                rApi.operations.Add(rApiOperation);

                foreach (var param in api.ParameterDescriptions)
                {
                    var parameter = _swaggerFactory.CreateOperationParam(api, param, _docProvider);
                    rApiOperation.parameters.Add(parameter);
                    var paramTypeName = param.ParameterDescriptor.ParameterType.FullName;
                    
                    if(paramTypeName!=null && !apiModels.ContainsKey(paramTypeName))
                    {
                        var apiModel = _docProvider.GetApiModel(param.ParameterDescriptor.ParameterType);
                        apiModels.Add(paramTypeName, apiModel);    
                    }
                }

          //      r.models = apiModels.Values.ToList();

            }
            return r;
        }
    }
}
