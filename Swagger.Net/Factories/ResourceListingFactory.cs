using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Description;

namespace Swagger.Net.Factories
{
    public interface IResourceListingFactory
    {
        ResourceListing CreateResourceListing(Uri uri, string controllerName, IEnumerable<ApiDescription> apiDescs);
        IList<ResourceSummary> CreateApiElements(IEnumerable<ApiDescription> apiDescs);
    }

    public class ResourceListingFactory : IResourceListingFactory
    {
        private readonly string _appVirtualPath;

        public ResourceListingFactory():this(HttpRuntime.AppDomainAppVirtualPath){}

        public ResourceListingFactory(string appVirtualPath)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/');
        }

        public ResourceListing CreateResourceListing(Uri uri, string controllerName, IEnumerable<ApiDescription> apiDescs)
        {
            var apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();
            var apis = CreateApiElements(apiDescs);

            var rtnListing = new ResourceListing()
            {
                apiVersion = apiVersion,
                swaggerVersion = SwaggerConstants.SWAGGER_VERSION,
                basePath = uri.GetLeftPart(UriPartial.Authority) + _appVirtualPath,
                resourcePath = controllerName,
                apis = apis
            };

            return rtnListing;
        }

        public IList<ResourceSummary> CreateApiElements(IEnumerable<ApiDescription> apiDescs)
        {
            var rtnApis = new Dictionary<String, ResourceSummary>();

            foreach (var desc in apiDescs)
            {
                var ctlrName = desc.ActionDescriptor.ControllerDescriptor.ControllerName;

                if(!rtnApis.ContainsKey(ctlrName))
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
    }
}
