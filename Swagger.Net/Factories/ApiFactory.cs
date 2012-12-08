using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// <summary>
    /// | .net              | swagger           |
    /// -----------------------------------------
    /// | ApiDescription    | Resource          |
    /// </summary>
    public class ApiFactory
    {

        #region --- fields & ctors ---

        private readonly string _appVirtualPath;
        private readonly XmlCommentDocumentationProvider _docProvider;
        private readonly ParameterFactory _parameterFactory;
        private readonly ICollection<ApiDescription> _apiDescriptions;
        private readonly ModelFactory _modelFactory;

        public ApiFactory()
        {
            _appVirtualPath = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/'); ;
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
            _parameterFactory = new ParameterFactory();

            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
            _modelFactory = new ModelFactory(_docProvider);
        }

        public ApiFactory(string virtualPath, XmlCommentDocumentationProvider docProvider, ParameterFactory parameterFactory, ModelFactory modelFactory, ICollection<ApiDescription> apiDescriptions)
        {
            _apiDescriptions = apiDescriptions;
            _modelFactory = modelFactory;
            _parameterFactory = parameterFactory;
            _appVirtualPath = virtualPath.TrimEnd('/');
            _docProvider = docProvider;
        }

        #endregion --- fields & ctors ---

        public IOrderedEnumerable<ApiDeclaration> CreateAllApiDeclarations(string rootUrl)
        {

            var uniqueControllers = _apiDescriptions
                .Select(api => api.ActionDescriptor.ControllerDescriptor)
                .Distinct();

            var rtnApis = uniqueControllers
                .Select(controller => CreateApiDeclaration(rootUrl, controller));
            
            return rtnApis.OrderBy(api => api.resourcePath);
                
        }

        

        public ApiDeclaration CreateApiDeclaration(string rootUrl, HttpControllerDescriptor controller)
        {
            var rootCtlrName = controller.ControllerName;
            
            var apiDescripts = GetFilteredApiDescriptions(rootCtlrName);

            var dataModels = _modelFactory.GetModels(apiDescripts);
            var apis = CreateApis(apiDescripts, rootCtlrName);

            var docs = _docProvider.GetDocumentation(controller.ControllerType);

            return new ApiDeclaration()
            {
                description = docs,
                swaggerVersion = G.SWAGGER_VERSION,
                basePath = rootUrl + _appVirtualPath,
                resourcePath = controller.ControllerName,
                apis = apis.ToList(),
                models = dataModels,
                apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString()
            };
        }


        public IEnumerable<Models.Api> CreateApis(IEnumerable<ApiDescription> apiDescs, string rootControllerName)
        {
            var rtnApis = new List<Models.Api>();
            foreach (var apiDescription in apiDescs)
            {
                var operations = CreateOperation(apiDescription);
                var currentControllerName = apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName;
                var alternatePath = PpsUtil.GetAlternatePath(rootControllerName, currentControllerName, apiDescription.RelativePath);

                Debug.WriteLine(currentControllerName + ":" + apiDescription.HttpMethod + ":" + alternatePath.ToLower() + ":" + apiDescription.RelativePath.ToLower());

                rtnApis.Add(new Models.Api()
                          {
                              path = alternatePath,
                              description = apiDescription.Documentation,
                              operations = operations,
                          });
            }
            return rtnApis;
        }

        public ApiOperation[] CreateOperation(ApiDescription apiDesc)
        {
            var responseClass = CalculateResponseClass(apiDesc.ActionDescriptor.ReturnType);
            var remarks = _docProvider.GetRemarks(apiDesc.ActionDescriptor);
            var parameters = _parameterFactory.CreateParameters(apiDesc.ParameterDescriptions, apiDesc.RelativePath);
            var rApiOperation = new ApiOperation()
            {
                httpMethod = apiDesc.HttpMethod.ToString(),
                nickname = apiDesc.ActionDescriptor.ActionName,
                responseClass = responseClass,
                summary = apiDesc.Documentation,
                notes = remarks,
                parameters = parameters
            };

            return new[] { rApiOperation };
        }

        public ApiDeclaration CreateApiDeclaration(string rootUrl, string controllerName)
        {
            var apiDescription = _apiDescriptions
                .FirstOrDefault(api => api.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == controllerName.ToLower());

            if (apiDescription == null) return null;

            var rtnApiDeclare = CreateApiDeclaration(rootUrl, apiDescription.ActionDescriptor.ControllerDescriptor);

            return rtnApiDeclare;
        }

        private List<ApiDescription> GetFilteredApiDescriptions(string rootControllerName)
        {
            var filteredDescs =
                _apiDescriptions
                    .Where(d =>
                        d.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower().StartsWith(rootControllerName.ToLower())
                     );

            return filteredDescs.OrderBy(api => api.ActionDescriptor.ControllerDescriptor.ControllerName).ToList();
        }

        private static string CalculateResponseClass(Type type)
        {
            if (type == null) return "void";

            return type.IsGenericType ? type.GetGenericArguments().First().Name : type.Name;
        }


    }
}
