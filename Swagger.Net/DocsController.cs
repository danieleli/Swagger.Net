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
        private readonly ResourceAdapter _resourceAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocsController"/> class.
        /// </summary>
        public DocsController()
        {
            _resourceAdapter = new ResourceAdapter();
            _apiAdapter = new ApiAdapter();
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocsController"/> class.
        /// </summary>
        public DocsController(ResourceAdapter resourceAdapter, ApiAdapter apiAdapter)
        {
            _resourceAdapter = resourceAdapter;
            _apiAdapter = apiAdapter;  
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
            // Arrange
            var uri = base.ControllerContext.Request.RequestUri;

            // Act
            var resourceListing = _resourceAdapter.CreateResourceListing(uri);

            //Answer
            var resp = WrapResponse(resourceListing);
            return resp;
        }

        public HttpResponseMessage Get(string id)
        {
            // Arrange
            var rootUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

            // Act
            var docs = _apiAdapter.GetDocs(rootUrl, id);

            //Answer
            return WrapResponse(docs);
        }

        private HttpResponseMessage WrapResponse<T>(T resourceListing)
        {
            var content = FormatContent<T>(resourceListing);

            var resp = new HttpResponseMessage {Content = content};
            return resp;
        }

        private ObjectContent<T> FormatContent<T>(T resourceListing)
        {
            var formatter = ControllerContext.Configuration.Formatters.JsonFormatter;
            var content = new ObjectContent<T>(resourceListing, formatter);
            return content;
        }
    }
}