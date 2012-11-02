using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Swagger.Net.Factories;
using Swagger.Net.Models;

namespace Swagger.Net
{

    public class SwaggerController : ApiController
    {
        private readonly IEnumerable<ApiDescription> _apiDescriptions;
        private readonly IResourceListingFactory _resourceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerController"/> class.
        /// </summary>
        public SwaggerController()
        {
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
            _resourceFactory = new ResourceListingFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerController"/> class.
        /// </summary>
        /// <param name="apiDescriptions">The apiExplorer to use.</param>
        /// <param name="docProvider">The xmlDocProvider to use.</param>
        public SwaggerController(IEnumerable<ApiDescription> apiDescriptions, IDocumentationProvider docProvider, IResourceListingFactory resourceFactory)
        {
            _apiDescriptions = apiDescriptions;
            _resourceFactory = resourceFactory;
        }

        /// <summary>
        /// Get the resource description of the api for swagger documentation
        /// </summary>
        /// <remarks>It is very convenient to have this information available for generating clients. This is the entry point for the swagger UI
        /// </remarks>
        /// <returns>JSON document representing structure of API</returns>
        public HttpResponseMessage Get()
        {
            var uri = base.ControllerContext.Request.RequestUri;
            var controllerName = base.ControllerContext.ControllerDescriptor.ControllerName;
            var resourceListing = _resourceFactory.CreateResourceListing(uri, controllerName, _apiDescriptions);
            
            var formatter = ControllerContext.Configuration.Formatters.JsonFormatter;
            var content = new ObjectContent<ResourceListing>(resourceListing, formatter);
            
            var resp = new HttpResponseMessage { Content = content };
            return resp;
        }
    }
}