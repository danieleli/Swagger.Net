using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http.Description;
using Swagger.Net.Models;

namespace Swagger.Net
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
            var rtnListing = new ResourceListing()
            {
                apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString(),
                swaggerVersion = SwaggerConstants.SWAGGER_VERSION,
                basePath = uri.GetLeftPart(UriPartial.Authority) + _appVirtualPath,
                resourcePath = controllerName,
                apis = CreateApiElements(apiDescs)
                
            };

            return rtnListing;
        }


        public IList<ResourceSummary> CreateApiElements(IEnumerable<ApiDescription> apiDescs)
        {
            var rtnApis = new Dictionary<String, ResourceSummary>();
         
            var apiSummaries = apiDescs.Select(a => new
                                            {
                                                a.ActionDescriptor.ControllerDescriptor.ControllerName,
                                                a.RelativePath,
                                                a.Documentation
                                            });

            foreach (var apiSum in apiSummaries)
            {
                if(!rtnApis.ContainsKey(apiSum.ControllerName))
                {
                    var res = new ResourceSummary
                    {
                        path = "/" + apiSum.RelativePath,
                        description = apiSum.Documentation
                    };
                    rtnApis.Add(apiSum.ControllerName, res);    
                }
            }

            return rtnApis.Values.ToList();
        }
    }
}
