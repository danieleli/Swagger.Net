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
        
        private readonly IEnumerable<ApiDescription> _apiDescriptions;
        private readonly ResourceMetadataFactory _factory;

        public SwaggerActionFilterAttribute()
        {
            _factory = new ResourceMetadataFactory();
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
        }

        public SwaggerActionFilterAttribute(IEnumerable<ApiDescription> apiDescriptions, ResourceMetadataFactory factory)
        {
            _apiDescriptions = apiDescriptions;
            _factory = factory;
        }

        #endregion // --- fields & ctors ---

        // Intercept all request and handle swagger requests or pass through
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!IsDocRequest(actionContext)) return;

            var docs = GetDocs(actionContext);
            var formatter = actionContext.ControllerContext.Configuration.Formatters.JsonFormatter;
            var response = WrapResponse(formatter, docs);
            actionContext.Response = response;
        }

        #region --- helpers ---

        private bool IsDocRequest(HttpActionContext actionContext)
        {
            var containsKey = actionContext.ControllerContext.RouteData.Values.ContainsKey(G.SWAGGER);

            if (!containsKey)
            {
                base.OnActionExecuting(actionContext);
                return false;
            }
            return true;
        }

        private ResourceDescription GetDocs(HttpActionContext actionContext)
        {
            var uri = actionContext.Request.RequestUri;
            var ctlrName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;

            var docs = _factory.CreateResourceMetadata(uri, ctlrName);
            var filteredApiDescs = FilterApis(ctlrName);
            var apis = _factory.CreateApiElements(filteredApiDescs);
            docs.apis.AddRange(apis);

            var models = GetModels(filteredApiDescs);

            docs.models.AddRange(models);
            return docs;
        }

        
        private IEnumerable<ApiDescription> FilterApis(string controllerName)
        {
            var filteredDescs = _apiDescriptions
                .Where(d => d.ActionDescriptor.ControllerDescriptor.ControllerName == controllerName)           // current controller
                .Where(d => !(d.Route.Defaults.ContainsKey(G.SWAGGER)));                                        // and not swagger doc meta route '/api/docs/...'

            return filteredDescs;
        }

        private IEnumerable<Model> GetModels(IEnumerable<ApiDescription> apiDescs)
        {
            var rtnModels = new Dictionary<Type,Model>();
            foreach (var apiDesc in apiDescs)
            {
                var returnType = apiDesc.ActionDescriptor.ReturnType;
                if (returnType !=null && !rtnModels.ContainsKey(returnType))
                {
                    var returnModel = _factory.GetResourceModel(returnType);
                    rtnModels.Add(returnType, returnModel);    
                }
                
                foreach (var param in apiDesc.ParameterDescriptions)
                {
                    var paramType = param.ParameterDescriptor.ParameterType;
                    if (!rtnModels.ContainsKey(paramType))
                    {
                        var paramModel = _factory.GetResourceModel(paramType);
                        rtnModels.Add(paramType, paramModel);
                    }
                }
            }
            
            return rtnModels.Values;
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
