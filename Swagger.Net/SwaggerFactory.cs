using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using Swagger.Net.Models;

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
        Api CreateResourceApi(ApiDescription api);

        /// <summary>
        /// Creates an api operation
        /// </summary>
        /// <param name="api">Description of the api via the ApiExplorer</param>
        /// <param name="docProvider">Access to the XML docs written in code</param>
        /// <returns>An api operation</returns>
        Operation CreateApiOperation(ApiDescription api, XmlCommentDocumentationProvider docProvider);

        /// <summary>
        /// Creates an operation parameter
        /// </summary>
        /// <param name="api">Description of the api via the ApiExplorer</param>
        /// <param name="param">Description of a parameter on an operation via the ApiExplorer</param>
        /// <param name="docProvider">Access to the XML docs written in code</param>
        /// <returns>An operation parameter</returns>
        Parameter CreateOperationParam(ApiDescription api, ApiParameterDescription param, XmlCommentDocumentationProvider docProvider);
    }

    public class SwaggerFactory : ISwaggerFactory
    {
        public ResourceListing CreateResourceListing(HttpActionContext actionContext, bool includeResourcePath = true)
        {
            return CreateResourceListing(actionContext.ControllerContext, includeResourcePath);
        }

        public ResourceListing CreateResourceListing(HttpControllerContext controllerContext, bool includeResourcePath = false)
        {
            var uri = controllerContext.Request.RequestUri;

            var rl = new ResourceListing()
                    {
                        apiVersion = Assembly.GetCallingAssembly().GetType().Assembly.GetName().Version.ToString(),
                        swaggerVersion = SwaggerConstants.SWAGGER_VERSION,
                        basePath = uri.GetLeftPart(UriPartial.Authority) + HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/'),
                    
                    };

            if (includeResourcePath) rl.resourcePath = controllerContext.ControllerDescriptor.ControllerName;

            return rl;
        }

        public Api CreateResourceApi(ApiDescription api)
        {
            var rApi = new Api()
                                   {
                                       path = "/" + api.RelativePath,
                                       description = api.Documentation,
                                       operations = new List<Operation>()
                                   };

            return rApi;
        }

        public Operation CreateApiOperation(ApiDescription api, XmlCommentDocumentationProvider docProvider)
        {
            var rApiOperation = new Operation()
                                            {
                                                httpMethod = api.HttpMethod.ToString(),
                                                nickname = docProvider.GetNickname(api.ActionDescriptor),
                                                responseClass = docProvider.GetResponseClass(api.ActionDescriptor),
                                                summary = api.Documentation,
                                                notes = docProvider.GetNotes(api.ActionDescriptor),
                                                parameters = new List<Parameter>()
                                            };

            return rApiOperation;
        }

        public Parameter CreateOperationParam(ApiDescription api,
                                                    ApiParameterDescription param,
                                                    XmlCommentDocumentationProvider docProvider)
        {
            var paramType = SwaggerConstants.BODY;
            if(param.Source.ToString().Equals(SwaggerConstants.FROMURI))
            {
                if(api.RelativePath.IndexOf("{" + param.Name + "}") > -1)
                {
                    paramType = SwaggerConstants.PATH;
                }
                else
                {
                    paramType = SwaggerConstants.QUERY;
                }
                
            }

            var parameter = new Parameter()
                                        {
                                            paramType = paramType,
                                            name = param.Name,
                                            description = param.Documentation,
                                            dataType = param.ParameterDescriptor.ParameterType.Name,
                                            required = docProvider.GetRequired(param.ParameterDescriptor)
                                        };

            return parameter;
        }
    }
}