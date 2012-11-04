using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Swagger.Net.Factories
{
    public interface IEndpointMetadataFactory
    {
        ResourceListing CreateResourceListing(Uri uri);
        IList<ResourceSummary> CreateApiElements(IEnumerable<ApiDescription> apiDescs);
    }

    public class EndpointMetadataFactory : IEndpointMetadataFactory
    {

        #region --- fields & ctors ---

        private string _appVirtualPath;
        private IEnumerable<ApiDescription> _apiDescriptions;

        public EndpointMetadataFactory()
        {
            Init(HttpRuntime.AppDomainAppVirtualPath, GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        public EndpointMetadataFactory(string appVirtualPath, IEnumerable<ApiDescription> apiDescs)
        {
            Init(appVirtualPath, apiDescs);
        }

        private void Init(string appVirtualPath, IEnumerable<ApiDescription> apiDescs)
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

        public IList<ResourceSummary> CreateApiElements(IEnumerable<ApiDescription> apiDescs)
        {
            var rtnApis = new Dictionary<String, ResourceSummary>();

            foreach (var desc in apiDescs)
            {
                var ctlrName = desc.ActionDescriptor.ControllerDescriptor.ControllerName;

                // skip swagger controller and items already in dictionary.
                if (IsSwaggerController(ctlrName) || rtnApis.ContainsKey(ctlrName))
                {
                    // do nothing
                }
                else
                {
                    var res = new ResourceSummary
                    {
                        path = "/" + desc.RelativePath,
                        description = desc.Documentation
                    };
                    rtnApis.Add(ctlrName, res);
                }
            }

            return rtnApis.Values.ToList();
        }

        private static bool IsSwaggerController(string ctlrName)
        {
            return ctlrName.ToUpper() == G.SWAGGER.ToUpper();
        }
    }
}
