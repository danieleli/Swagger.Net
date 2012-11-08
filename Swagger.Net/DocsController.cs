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

        private readonly ApiAdapter _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerController"/> class.
        /// </summary>
        public DocsController()
        {
            _factory = new ApiAdapter();  
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerController"/> class.
        /// </summary>
        public DocsController(ApiAdapter resourceFactory)
        {
            _factory = resourceFactory;  
        }

        #endregion --- fields & ctors ---

      

        public HttpResponseMessage Get(string id)
        {
            // Arrange
            var rootUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

            // Act
            var docs = _factory.GetDocs(rootUrl, id);

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