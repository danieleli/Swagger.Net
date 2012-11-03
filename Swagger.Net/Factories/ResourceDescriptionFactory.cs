using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    public interface IResourceDescriptionFactory
    {
        ResourceDescription CreateResourceDescription(Uri uri, string controllerName);
        IList<Api> CreateApiElements(string controllerName, IEnumerable<ApiDescription> descriptions);
        IList<Operation> CreateOperations(ApiDescription desc, XmlCommentDocumentationProvider docProvider);
        IList<Parameter> CreateParameters(Collection<HttpParameterDescriptor> httpParams);
    }

    public class ResourceDescriptionFactory : IResourceDescriptionFactory
    {

        private readonly string _appVirtualPath;
        public ResourceDescriptionFactory() : this(HttpRuntime.AppDomainAppVirtualPath) { }
        public ResourceDescriptionFactory(string appVirtualPath)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/');
        }

        public ResourceDescription CreateResourceDescription(Uri uri, string controllerName)
        {

            var rtnResource = new ResourceDescription()
            {
                apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString(),
                swaggerVersion = SwaggerConstants.SWAGGER_VERSION,
                basePath = uri.GetLeftPart(UriPartial.Authority) + _appVirtualPath,
                resourcePath = controllerName
            };

            return rtnResource;
        }

        public IList<Api> CreateApiElements(string controllerName, IEnumerable<ApiDescription> descriptions)
        {
            var rtnApis = new List<Api>();

            if (controllerName.ToUpper() == SwaggerConstants.SWAGGER.ToUpper()) return rtnApis;

            // apis for current controller
            var filteredDescs = descriptions.Where(d =>
                    d.ActionDescriptor.ControllerDescriptor.ControllerName == controllerName
            );

            foreach (var desc in filteredDescs)
            {
                var api = new Api()
                {
                    path = "/" + desc.RelativePath,
                    description = desc.Documentation,
                    operations = CreateOperations(desc, null)
                };

                // todo: add operations;=

                rtnApis.Add(api);


            }
            return rtnApis;
        }

        public IList<Operation> CreateOperations(ApiDescription desc, XmlCommentDocumentationProvider docProvider)
        {
            var returnType = desc.ActionDescriptor.ReturnType == null ? "void" : desc.ActionDescriptor.ReturnType.Name;
            var rApiOperation = new Operation()
            {
                httpMethod = desc.HttpMethod.ToString(),
                nickname = desc.ActionDescriptor.ActionName,
                responseClass = returnType,
                summary = desc.Documentation,
                notes = "notes",//docProvider.GetNotes(api.ActionDescriptor),
                parameters = CreateParameters(desc.ActionDescriptor.GetParameters())
            };

            return new List<Operation>() { rApiOperation };
        }

        public IList<Parameter> CreateParameters(Collection<HttpParameterDescriptor> httpParams)
        {
            var rtn = new List<Parameter>();
            foreach (var p in httpParams)
            {
                rtn.Add(new Parameter()
                            {
                                name = p.ParameterName,
                                dataType = p.ParameterType.Name,
                                required = !p.IsOptional
                            });
            }

            return rtn;
        }
    }
}

