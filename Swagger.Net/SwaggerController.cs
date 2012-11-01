using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Swagger.Net
{

    public class SwaggerController : ApiController
    {
        private readonly IApiExplorer _apiExplorer;
        private IDocumentationProvider _docProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerController"/> class.
        /// </summary>
        public SwaggerController()
        {
            _apiExplorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();
            _docProvider = GlobalConfiguration.Configuration.Services.GetDocumentationProvider();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerController"/> class.
        /// </summary>
        /// <param name="apiExplorer">The apiExplorer to use.</param>
        /// <param name="docProvider">The xmlDocProvider to use.</param>
        public SwaggerController(IApiExplorer apiExplorer, IDocumentationProvider docProvider, ISwaggerFactory swaggerFactory)
        {
            _apiExplorer = apiExplorer;
            _docProvider = docProvider;
        }

        /// <summary>
        /// Get the resource description of the api for swagger documentation
        /// </summary>
        /// <remarks>It is very convenient to have this information available for generating clients. This is the entry point for the swagger UI
        /// </remarks>
        /// <returns>JSON document representing structure of API</returns>
        public HttpResponseMessage Get()
        {
            
            var controllerName = base.ControllerContext.ControllerDescriptor.ControllerName;
            var apiResourceListing = this.CreateResourceListing(controllerName);

            var uniqueControllers = new List<string>();

            foreach (var api in _apiExplorer.ApiDescriptions)
            {
                if (controllerName.ToUpper().Equals(SwaggerFactory.SWAGGER.ToUpper())) continue;
                if (uniqueControllers.Contains(controllerName)) continue;

                uniqueControllers.Add(controllerName);

                var rApi = this.CreateResourceApi(api.RelativePath, api.Documentation);
                apiResourceListing.apis.Add(rApi);
            }

            var content = new ObjectContent<ResourceListing>(apiResourceListing, ControllerContext.Configuration.Formatters.JsonFormatter);
            var resp = new HttpResponseMessage()
                           {
                               Content = content
                           };

            return resp;
        }

        private ResourceListing CreateResourceListing(string controllerName)
        {
            var uri = base.ControllerContext.Request.RequestUri;
            var appDomainVirtualPath = HttpRuntime.AppDomainAppVirtualPath + "";
            
            var rtn = new ResourceListing()
            {
                apiVersion = Assembly.GetCallingAssembly().GetType().Assembly.GetName().Version.ToString(),
                swaggerVersion = SwaggerFactory.SWAGGER_VERSION,
                basePath = uri.GetLeftPart(UriPartial.Authority) + appDomainVirtualPath.TrimEnd('/'),
                apis = new List<ResourceApi>(),
                resourcePath = controllerName
            };

            return rtn;
        }

        public ResourceApi CreateResourceApi(string relativePath, string documentation)
        {
            if (!relativePath.StartsWith("/"))
            {
                relativePath = "/" + relativePath;
            }
            var rApi = new ResourceApi()
            {
                path = relativePath,
                description = documentation,
                operations = new List<ResourceApiOperation>()
            };

            return rApi;
        }
    }
}
