using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace Swagger.Net
{
    public static class SwaggerGen
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
        public static ResourceListing CreateResourceListing(HttpActionContext actionContext, bool includeResourcePath = true)
        {
            return CreateResourceListing(actionContext.ControllerContext, includeResourcePath);
        }

        /// <summary>
        /// Create a resource listing
        /// </summary>
        /// <param name="actionContext">Current controller context</param>
        /// <param name="includeResourcePath">Should the resource path property be included in the response</param>
        /// <returns>A resrouce listing</returns>
        public static ResourceListing CreateResourceListing(HttpControllerContext controllerContext, bool includeResourcePath = false)
        {
            Uri uri = controllerContext.Request.RequestUri;

            ResourceListing rl = new ResourceListing()
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
        public static ResourceApi CreateResourceApi(ApiDescription api)
        {
            ResourceApi rApi = new ResourceApi()
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
        public static ResourceApiOperation CreateResourceApiOperation(ApiDescription api, XmlCommentDocumentationProvider docProvider)
        {
            ResourceApiOperation rApiOperation = new ResourceApiOperation()
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
        public static ResourceApiOperationParameter CreateResourceApiOperationParameter(ApiDescription api, ApiParameterDescription param, XmlCommentDocumentationProvider docProvider)
        {
            string paramType = (param.Source.ToString().Equals(FROMURI)) ? QUERY : BODY;
            ResourceApiOperationParameter parameter = new ResourceApiOperationParameter()
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