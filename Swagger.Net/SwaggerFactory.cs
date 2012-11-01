using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace Swagger.Net
{
    public interface ISwaggerFactory
    {
        /// <summary>
        /// Create a resource listing
        /// </summary>
        /// <param name="actionContext">Current action context</param>
        /// <param name="includeResourcePath">Should the resource path property be included in the response</param>
        /// <returns>A resource Listing</returns>
        ResourceListing CreateResourceListing(HttpActionContext actionContext, bool includeResourcePath = true);

        /// <summary>
        /// Create a resource listing
        /// </summary>
        /// <param name="actionContext">Current controller context</param>
        /// <param name="includeResourcePath">Should the resource path property be included in the response</param>
        /// <returns>A resrouce listing</returns>
        ResourceListing CreateResourceListing(HttpControllerContext controllerContext, bool includeResourcePath = false);

        /// <summary>
        /// Create an api element 
        /// </summary>
        /// <param name="api">Description of the api via the ApiExplorer</param>
        /// <returns>A resource api</returns>
        ResourceApi CreateResourceApi(ApiDescription api);

        /// <summary>
        /// Creates an api operation
        /// </summary>
        /// <param name="api">Description of the api via the ApiExplorer</param>
        /// <param name="docProvider">Access to the XML docs written in code</param>
        /// <returns>An api operation</returns>
        ResourceApiOperation CreateResourceApiOperation(ApiDescription api, XmlCommentDocumentationProvider docProvider);

        /// <summary>
        /// Creates an operation parameter
        /// </summary>
        /// <param name="api">Description of the api via the ApiExplorer</param>
        /// <param name="param">Description of a parameter on an operation via the ApiExplorer</param>
        /// <param name="docProvider">Access to the XML docs written in code</param>
        /// <returns>An operation parameter</returns>
        ResourceApiOperationParameter CreateResourceApiOperationParameter(ApiDescription api, ApiParameterDescription param, XmlCommentDocumentationProvider docProvider);
    }

    public class SwaggerFactory : ISwaggerFactory
    {
        public const string SWAGGER = "swagger";
        public const string SWAGGER_VERSION = "2.0";
        public const string FROMURI = "FromUri";
        public const string FROMBODY = "FromBody";
        public const string QUERY = "query";
        public const string PATH = "path";
        public const string BODY = "body";

        /// <summary>
        /// Create a resource listing
        /// </summary>
        /// <param name="actionContext">Current action context</param>
        /// <param name="includeResourcePath">Should the resource path property be included in the response</param>
        /// <returns>A resource Listing</returns>
        public ResourceListing CreateResourceListing(HttpActionContext actionContext, bool includeResourcePath = true)
        {
            return CreateResourceListing(actionContext.ControllerContext, includeResourcePath);
        }

        /// <summary>
        /// Create a resource listing
        /// </summary>
        /// <param name="actionContext">Current controller context</param>
        /// <param name="includeResourcePath">Should the resource path property be included in the response</param>
        /// <returns>A resrouce listing</returns>
        public ResourceListing CreateResourceListing(HttpControllerContext controllerContext, bool includeResourcePath = false)
        {
            var uri = controllerContext.Request.RequestUri;

            var rl = new ResourceListing()
                                     {
                                         apiVersion = Assembly.GetCallingAssembly().GetType().Assembly.GetName().Version.ToString(),
                                         swaggerVersion = SWAGGER_VERSION,
                                         basePath = uri.GetLeftPart(UriPartial.Authority) + HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/'),
                                         apis = new List<ResourceApi>()
                                     };

            if (includeResourcePath) rl.resourcePath = controllerContext.ControllerDescriptor.ControllerName;

            return rl;
        }

        /// <summary>
        /// Create an api element 
        /// </summary>
        /// <param name="api">Description of the api via the ApiExplorer</param>
        /// <returns>A resource api</returns>
        public ResourceApi CreateResourceApi(ApiDescription api)
        {
            var rApi = new ResourceApi()
                                   {
                                       path = "/" + api.RelativePath,
                                       description = api.Documentation,
                                       operations = new List<ResourceApiOperation>()
                                   };

            return rApi;
        }

        /// <summary>
        /// Creates an api operation
        /// </summary>
        /// <param name="api">Description of the api via the ApiExplorer</param>
        /// <param name="docProvider">Access to the XML docs written in code</param>
        /// <returns>An api operation</returns>
        public ResourceApiOperation CreateResourceApiOperation(ApiDescription api, XmlCommentDocumentationProvider docProvider)
        {
            var rApiOperation = new ResourceApiOperation()
                                                     {
                                                         httpMethod = api.HttpMethod.ToString(),
                                                         nickname = docProvider.GetNickname(api.ActionDescriptor),
                                                         responseClass = docProvider.GetResponseClass(api.ActionDescriptor),
                                                         summary = api.Documentation,
                                                         notes = docProvider.GetNotes(api.ActionDescriptor),
                                                         parameters = new List<ResourceApiOperationParameter>()
                                                     };

            return rApiOperation;
        }

        /// <summary>
        /// Creates an operation parameter
        /// </summary>
        /// <param name="api">Description of the api via the ApiExplorer</param>
        /// <param name="param">Description of a parameter on an operation via the ApiExplorer</param>
        /// <param name="docProvider">Access to the XML docs written in code</param>
        /// <returns>An operation parameter</returns>
        public ResourceApiOperationParameter CreateResourceApiOperationParameter(ApiDescription api, ApiParameterDescription param, XmlCommentDocumentationProvider docProvider)
        {
            var paramType = (param.Source.ToString().Equals(FROMURI)) ? QUERY : BODY;
            var parameter = new ResourceApiOperationParameter()
                                                          {
                                                              paramType = (paramType == "query" && api.RelativePath.IndexOf("{" + param.Name + "}") > -1) ? PATH : paramType,
                                                              name = param.Name,
                                                              description = param.Documentation,
                                                              dataType = param.ParameterDescriptor.ParameterType.Name,
                                                              required = docProvider.GetRequired(param.ParameterDescriptor)
                                                          };

            return parameter;
        }
    }
}