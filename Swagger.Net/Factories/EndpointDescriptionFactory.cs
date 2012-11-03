using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Swagger.Net.Factories
{
    public interface IEndpointDescriptionFactory
    {
        ResourceListing CreateResourceListing(Uri uri, string controllerName);
        IList<ResourceSummary> CreateApiElements(IEnumerable<ApiDescription> apiDescs);
    }

    public class EndpointDescriptionFactory : IEndpointDescriptionFactory
    {
        private string _appVirtualPath;
        private IEnumerable<ApiDescription> _apiDescriptions;

        public EndpointDescriptionFactory()
        {
            Init(HttpRuntime.AppDomainAppVirtualPath, GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        public EndpointDescriptionFactory(string appVirtualPath, IEnumerable<ApiDescription> apiDescs)
        {
            Init(appVirtualPath, apiDescs);
        }

        private void Init(string appVirtualPath, IEnumerable<ApiDescription> apiDescs)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/');
            _apiDescriptions = apiDescs;
        }

        

        public ResourceListing CreateResourceListing(Uri uri, string controllerName)
        {
            var apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();
            
            var rtnListing = new ResourceListing()
            {
                apiVersion = apiVersion,
                swaggerVersion = SwaggerConstants.SWAGGER_VERSION,
                basePath = uri.GetLeftPart(UriPartial.Authority) + _appVirtualPath,
                resourcePath = controllerName,
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
