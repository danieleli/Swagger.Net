using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    public interface IResourceDescriptionFactory
    {
        ResourceDescription CreateResourceDescription(Uri uri, string controllerName);
        IList<Api> CreateApiElements(string controllerName, IEnumerable<ApiDescription> descriptions);
        IList<Operation> CreateOperations(ApiDescription desc);
        IList<Parameter> CreateParameters(Collection<ApiParameterDescription> httpParams);
        Api CreateApi(ApiDescription apiDesc);
        Parameter CreateParameter(ApiParameterDescription parameterDescription);
    }

    public class ResourceDescriptionFactory : IResourceDescriptionFactory
    {
        #region --- fields & ctors ---

        private string _appVirtualPath;
        private XmlCommentDocumentationProvider _docProvider;

        public ResourceDescriptionFactory()
        {
            var path = HttpRuntime.AppDomainAppVirtualPath;
            var docProvider = (IDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
            initialize(path, docProvider);
        }

        public ResourceDescriptionFactory(string virturalPath, XmlCommentDocumentationProvider docProvider)
        {
            initialize(virturalPath, docProvider);
        }

        public void initialize(string appVirtualPath, IDocumentationProvider docProvider)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/');
            _docProvider = (XmlCommentDocumentationProvider) docProvider;
        }

        #endregion --- fields & ctors ---

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
                var api = CreateApi(desc);
                rtnApis.Add(api);
            }

            return rtnApis;
        }

        public Api CreateApi(ApiDescription desc)
        {
            var ops = CreateOperations(desc);
            var api = new Api()
                          {
                              path = "/" + desc.RelativePath,
                              description = desc.Documentation,
                              operations = ops
                          };
            return api;
        }

        public IList<Operation> CreateOperations(ApiDescription desc)
        {

            var returnType = desc.ActionDescriptor.ReturnType == null ? "void" : desc.ActionDescriptor.ReturnType.Name;
            var paramtrs= CreateParameters(desc.ParameterDescriptions);
            var remarks = _docProvider.GetNotes(desc.ActionDescriptor);

            var rApiOperation = new Operation()
            {
                httpMethod = desc.HttpMethod.ToString(),
                nickname = desc.ActionDescriptor.ActionName,
                responseClass = returnType,
                summary = desc.Documentation,
                notes = remarks,
                parameters = paramtrs
            };

            return new List<Operation>() { rApiOperation };
        }

        public IList<Parameter> CreateParameters(Collection<ApiParameterDescription> httpParams)
        {
            var rtn = new List<Parameter>();
            foreach (var p in httpParams)
            {
                var param = CreateParameter(p);
                rtn.Add(param);
            }

            return rtn;
        }

        public Parameter CreateParameter(ApiParameterDescription p)
        {
            var rtn = new Parameter()
                            {
                                name = p.Name,
                                dataType = p.ParameterDescriptor.ParameterType.Name,
                                required = !p.ParameterDescriptor.IsOptional,
                                description = p.Documentation,
                                paramType = p.Source.ToString()
                                // allowMultiple = p.ParameterDescriptor.
                                // allowableValues
                            };
            return rtn;
        }

    }
}

