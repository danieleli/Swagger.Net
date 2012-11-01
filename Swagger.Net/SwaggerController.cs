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
        private readonly ISwaggerFactory _swaggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerController"/> class.
        /// </summary>
        public SwaggerController()
        {
            _apiExplorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();
            _docProvider = GlobalConfiguration.Configuration.Services.GetDocumentationProvider();
            _swaggerFactory = new SwaggerFactory();
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
            _swaggerFactory = swaggerFactory;
        }

        /// <summary>
        /// Get the resource description of the api for swagger documentation
        /// </summary>
        /// <remarks>It is very convenient to have this information available for generating clients. This is the entry point for the swagger UI
        /// </remarks>
        /// <returns>JSON document representing structure of API</returns>
        public HttpResponseMessage Get()
        {
            ResourceListing r = _swaggerFactory.CreateResourceListing(base.ControllerContext);
            List<string> uniqueControllers = new List<string>();

            foreach (var api in _apiExplorer.ApiDescriptions)
            {
                string controllerName = api.ActionDescriptor.ControllerDescriptor.ControllerName;
                if (uniqueControllers.Contains(controllerName) ||
                      controllerName.ToUpper().Equals(SwaggerFactory.SWAGGER.ToUpper())) continue;

                uniqueControllers.Add(controllerName);

                ResourceApi rApi = _swaggerFactory.CreateResourceApi(api);
                r.apis.Add(rApi);
            }

            HttpResponseMessage resp = new HttpResponseMessage();

            resp.Content = new ObjectContent<ResourceListing>(r, ControllerContext.Configuration.Formatters.JsonFormatter);            
            
            return resp;
        }
    }
}
