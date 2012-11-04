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
        #region --- fields & ctors ---

        private readonly IEndpointMetadataFactory _resourceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerController"/> class.
        /// </summary>
        public SwaggerController()
        {
            _resourceFactory = new EndpointMetadataFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerController"/> class.
        /// </summary>
        public SwaggerController(IEndpointMetadataFactory resourceFactory)
        {
            _resourceFactory = resourceFactory;
        }

        #endregion --- fields & ctors ---

        /// <summary>
        /// Get the resource description of the api for swagger documentation
        /// </summary>
        /// <remarks>It is very convenient to have this information available for generating clients. This is the entry point for the swagger UI
        /// </remarks>
        /// <returns>JSON document representing structure of API</returns>
        public HttpResponseMessage Get()
        {
            var resourceListing = GetResourceListing();
            var resp = WrapResponse(resourceListing);
            return resp;
        }

        private HttpResponseMessage WrapResponse(ResourceListing resourceListing)
        {
            var content = FormatContent(resourceListing);

            var resp = new HttpResponseMessage {Content = content};
            return resp;
        }

        private ObjectContent<ResourceListing> FormatContent(ResourceListing resourceListing)
        {
            var formatter = ControllerContext.Configuration.Formatters.JsonFormatter;
            var content = new ObjectContent<ResourceListing>(resourceListing, formatter);
            return content;
        }

        private ResourceListing GetResourceListing()
        {
            var uri = base.ControllerContext.Request.RequestUri;

            var resourceListing = _resourceFactory.CreateResourceListing(uri);
            return resourceListing;
        }
    }
}