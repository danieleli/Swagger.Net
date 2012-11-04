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
        IList<Operation> CreateOperations(ApiDescription apiDesc);
        IList<Parameter> CreateParameters(Collection<ApiParameterDescription> httpParams, string relativePath);
        Api CreateApi(ApiDescription apiDesc);
        Parameter CreateParameter(ApiParameterDescription parameterDescription, string relativePath);
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

        public ResourceDescriptionFactory(string virtualPath, XmlCommentDocumentationProvider docProvider)
        {
            initialize(virtualPath, docProvider);
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

            if (controllerName.ToUpper() == SwaggerConstants.SWAGGER.ToUpper())
            {
                return rtnApis;
            }

            // apis for current controller
            var filteredDescs = descriptions.Where(d =>
                    d.ActionDescriptor.ControllerDescriptor.ControllerName == controllerName
            );

            foreach (var desc in filteredDescs)
            {
                if (desc.Route.Defaults.ContainsKey(SwaggerConstants.SWAGGER))
                {
                    //skip
                }else
                {
                    var api = CreateApi(desc);
                    rtnApis.Add(api);    
                }
            }

            return rtnApis;
        }

        public Api CreateApi(ApiDescription desc)
        {
            var api = new Api()
                          {
                              path = "/" + desc.RelativePath,
                              description = desc.Documentation
                          };

            var ops = CreateOperations(desc);
            foreach (var operation in ops)
            {
                api.operations.Add(operation);
            }
            return api;
        }

        public IList<Operation> CreateOperations(ApiDescription apiDesc)
        {

            var returnType = apiDesc.ActionDescriptor.ReturnType == null ? "void" : apiDesc.ActionDescriptor.ReturnType.Name;
            
            var remarks = _docProvider.GetRemarks(apiDesc.ActionDescriptor);
            var rApiOperation = new Operation()
            {
                httpMethod = apiDesc.HttpMethod.ToString(),
                nickname = apiDesc.ActionDescriptor.ActionName,
                responseClass = returnType,
                summary = apiDesc.Documentation,
                notes = remarks,
            };

            var paramtrs = CreateParameters(apiDesc.ParameterDescriptions, apiDesc.RelativePath);
            foreach (var parameter in paramtrs)
            {
                rApiOperation.parameters.Add(parameter);
            }

            return new List<Operation>() { rApiOperation };
        }

        public IList<Parameter> CreateParameters(Collection<ApiParameterDescription> httpParams, string relativePath)
        {
            var rtn = new List<Parameter>();
            foreach (var p in httpParams)
            {
                var param = CreateParameter(p, relativePath);
                rtn.Add(param);
            }

            return rtn;
        }

        public Parameter CreateParameter(ApiParameterDescription parameterDescription, string relativePath)
        {
            

            var paramType = SwaggerConstants.BODY;
            if (parameterDescription.Source == ApiParameterSource.FromUri)
            {
                
                if (relativePath.IndexOf("{" + parameterDescription.Name + "}") > -1)
                {
                    paramType = SwaggerConstants.PATH;
                }
                else
                {
                    paramType = SwaggerConstants.QUERY;
                }

            }


            var isRequired = !parameterDescription.ParameterDescriptor.IsOptional;
                
            var rtn = new Parameter()
                            {
                                name = parameterDescription.Name,
                                dataType = parameterDescription.ParameterDescriptor.ParameterType.Name,
                                required = isRequired,
                                description = parameterDescription.Documentation,
                                paramType = paramType
                                // allowMultiple = p.ParameterDescriptor.
                                // allowableValues
                            };
            return rtn;
        }

    }
}

