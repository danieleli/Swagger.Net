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

        public ApiDeclaration[] CreateAllApiDeclarations(string root)
        {
            var uniqueControllers = _apiDescriptions
                .Select(api => api.ActionDescriptor.ControllerDescriptor)
                .Distinct(new ResourceComparer()).ToList();
            
            return uniqueControllers.Select(controller => CreateApiDeclaration(root, controller)).OrderBy(ctrl => ctrl.resourcePath).ToArray();
        }

        public ApiDeclaration CreateApiDeclaration(string root, string controllerName)
        {
            var myApi =
                _apiDescriptions.FirstOrDefault(api => api.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() == controllerName.ToLower());
            return CreateApiDeclaration(root, myApi.ActionDescriptor.ControllerDescriptor);
        }

        public ApiDeclaration CreateApiDeclaration(string root, HttpControllerDescriptor controller)
        {
            var apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();

            var apiDescriptions = GetApiDescriptions(controller.ControllerName);
            var apis = this.CreateApi(apiDescriptions);
            var models = _modelFactory.GetModels(apiDescriptions);

            return new ApiDeclaration()
            {
                description = _docProvider.GetDocumentation(controller.ControllerType),
                apiVersion = apiVersion,
                swaggerVersion = G.SWAGGER_VERSION,
                basePath = root + _appVirtualPath,
                resourcePath = controller.ControllerName,
                apis = apis.ToList(),
                models = models
            };
        }

        public IEnumerable<Models.Api> CreateApi(IEnumerable<ApiDescription> apiDescs)
        {
            var rtnApis = from apiDescription in apiDescs
                          let operations = CreateOperation(apiDescription)
                          select new Models.Api()
                          {
                              path = "/" + apiDescription.RelativePath,
                              description = apiDescription.Documentation,
                              operations = operations
                          };
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

        private List<ApiDescription> GetApiDescriptions(string controllerName)
        {
            var filteredDescs =
                _apiDescriptions
                    .Where(d =>
                        d.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower().StartsWith(controllerName.ToLower())
                     );

            return filteredDescs.OrderBy(api=>api.ActionDescriptor.ControllerDescriptor.ControllerName).ToList();
        }

        private static string CalculateResponseClass(Type type)
        {
            if (type == null) return "void";

            return type.IsGenericType ? type.GetGenericArguments().First().Name : type.Name;
        }


    }
}
