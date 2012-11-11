using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Swagger.Net.Factories;
using Swagger.Net.Models;

namespace Swagger.Net
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DocsController : ApiController
    {
        #region --- fields & ctors ---

        private readonly ApiAdapter _apiAdapter;
        private readonly ResourceListingFactory _resourceListingFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocsController"/> class.
        /// </summary>
        public DocsController()
        {
            _resourceListingFactory = new ResourceListingFactory();
            _apiAdapter = new ApiAdapter();
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocsController"/> class.
        /// </summary>
        public DocsController(ResourceListingFactory resourceListingFactory, ApiAdapter apiAdapter)
        {
            _resourceListingFactory = resourceListingFactory;
            _apiAdapter = apiAdapter;  
        }

        #endregion --- fields & ctors ---

        /// <summary>
        /// Get the list of resource descriptions (Models.ResourceListing) of the api for swagger documentation
        /// </summary>
        /// <remarks>
        /// It is very convenient to have this information available for generating clients. This is the entry point for the swagger UI
        /// </remarks>
        /// <returns>JSON document that lists resource urls and descriptions </returns>
        public HttpResponseMessage Get()
        {
            // Arrange
            var uri = base.ControllerContext.Request.RequestUri;

            // Act
            var resourceListing = _resourceListingFactory.CreateResourceListing(uri);

            //Answer
            var resp = WrapResponse(resourceListing);
            return resp;
        }

        public HttpResponseMessage Get(string id)
        {
            // Arrange
            var rootUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

            // Act
            var docs = _apiAdapter.CreateApiDeclaration(rootUrl, id);

            //Answer
            return WrapResponse(docs);
        }

        private HttpResponseMessage WrapResponse<T>(T resourceListing)
        {
            var formatter = ControllerContext.Configuration.Formatters.JsonFormatter;
            var content = new ObjectContent<T>(resourceListing, formatter);

            var resp = new HttpResponseMessage {Content = content};
            return resp;
        }

    }
}