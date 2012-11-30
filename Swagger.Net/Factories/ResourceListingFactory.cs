using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    /// <summary>
    /// | .net              | swagger           |
    /// -----------------------------------------
    /// | EndpointMetadata  | Resource Listing  |
    /// </summary>
    public class ResourceListingFactory
    {

        #region --- fields & ctors ---

        private readonly string _appVirtualPath;
        private readonly IEnumerable<ApiDescription> _apiDescriptions;

        public ResourceListingFactory()
        {
            _appVirtualPath = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
        }

        public ResourceListingFactory(string appVirtualPath, IEnumerable<ApiDescription> apiDescs)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/');
            _apiDescriptions = apiDescs;
        }

        #endregion --- fields & ctors ---

        public ResourceListing CreateResourceListing(Uri uri)
        {
            var apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();
            var basePath = uri.GetLeftPart(UriPartial.Authority) + _appVirtualPath;

            var rtnListing = new ResourceListing()
            {
                apiVersion = apiVersion,
                swaggerVersion = G.SWAGGER_VERSION,
                basePath = basePath + @"/api/docs",
                apis = CreateApis()
            };

            return rtnListing;
        }

        private Api[] CreateApis()
        {
            return (from api in _apiDescriptions
                    group api by api.ActionDescriptor.ControllerDescriptor.ControllerName
                    into g
                    select new Api
                               {
                                   description = g.First().Documentation,
                                   path = @"/" + g.Key
                               }
                   ).ToArray();
        }
    }
}
