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
    public class ResourceAdapter
    {

        #region --- fields & ctors ---

        private readonly string _appVirtualPath;
        private readonly IEnumerable<ApiDescription> _apiDescriptions;

        public ResourceAdapter()
        {
            _appVirtualPath = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
        }

        public ResourceAdapter(string appVirtualPath, IEnumerable<ApiDescription> apiDescs)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/'); 
            _apiDescriptions = apiDescs;
        }

        #endregion --- fields & ctors ---

        public ResourceListing CreateResourceListing(Uri uri)
        {
            var apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();

            var rtnListing = new ResourceListing()
            {
                apiVersion = apiVersion,
                swaggerVersion = G.SWAGGER_VERSION,
                basePath = uri.GetLeftPart(UriPartial.Authority) + _appVirtualPath,
            };

            
            var apis = CreateResourceElements(_apiDescriptions);
            
            foreach (var resourceSummary in apis)
            {
                rtnListing.apis.Add(resourceSummary);
            }

            return rtnListing;
        }

        public IList<Resource> CreateResourceElements(IEnumerable<ApiDescription> apiDescs)
        {
            var rtnApis = new Dictionary<String, Resource>();

            foreach (var desc in apiDescs)
            {
                var ctlrName = desc.ActionDescriptor.ControllerDescriptor.ControllerName;

                if (!rtnApis.ContainsKey(ctlrName))
                {
                    var res = new Resource
                    {
                        // todo: this is returning url with query string parameters only if first method has param(s)
                        path = GetPath(desc),
                        description = desc.Documentation
                    };
                    rtnApis.Add(ctlrName, res);
                }
            }

            return rtnApis.Values.ToList();
        }

        private static string GetPath(ApiDescription desc)
        {
            return "/api/docs/" + desc.ActionDescriptor.ControllerDescriptor.ControllerName;
            //string path;
            //var questionIndex = desc.RelativePath.IndexOf("?");
            //if (questionIndex < 1)
            //{
            //    path = desc.RelativePath;
            //}
            //else
            //{
            //    path = desc.RelativePath.Substring(0, questionIndex);
            //}
            //return path;
        }

        private bool IsSwaggerRoute(IHttpRoute route)
        {
            return route.Defaults.ContainsKey(G.SWAGGER);
        }

    }
}
