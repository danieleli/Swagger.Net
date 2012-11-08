﻿using System;
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
    public class EndpointMetadataFactory
    {

        #region --- fields & ctors ---

        private string _appVirtualPath;
        private IEnumerable<ApiDescription> _apiDescriptions;

        public EndpointMetadataFactory()
        {
            _appVirtualPath = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
        }

        public EndpointMetadataFactory(string appVirtualPath, IEnumerable<ApiDescription> apiDescs)
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

            
            var apis = CreateApiElements(_apiDescriptions);
            
            foreach (var resourceSummary in apis)
            {
                rtnListing.apis.Add(resourceSummary);
            }

            return rtnListing;
        }

        public IList<Api> CreateApiElements(IEnumerable<ApiDescription> apiDescs)
        {
            var rtnApis = new Dictionary<String, Api>();

            foreach (var desc in apiDescs)
            {
                var ctlrName = desc.ActionDescriptor.ControllerDescriptor.ControllerName;

                if (IsSwaggerRoute(desc.Route) && !rtnApis.ContainsKey(ctlrName))
                {
                    var res = new Api
                    {
                        // todo: this is returning url with query string parameters only if first method has param(s)
                        path = "/" + desc.RelativePath,
                        description = desc.Documentation
                    };
                    rtnApis.Add(ctlrName, res);
                }
            }

            return rtnApis.Values.ToList();
        }

        private bool IsSwaggerRoute(IHttpRoute route)
        {
            return route.Defaults.ContainsKey(G.SWAGGER);
        }

    }
}