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
    public class ApiAdapter
    {

        #region --- fields & ctors ---

        private readonly string _appVirtualPath;
        private readonly XmlCommentDocumentationProvider _docProvider;
        private readonly ParameterAdapter _parameterFactory;
        private readonly ICollection<ApiDescription> _apiDescriptions;
        private readonly ModelAdapter _modelFactory;

        public ApiAdapter()
        {
            _appVirtualPath = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/'); ;
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
            _parameterFactory = new ParameterAdapter();

            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
            _modelFactory = new ModelAdapter(_docProvider);
        }

        public ApiAdapter(string virtualPath, XmlCommentDocumentationProvider docProvider, ParameterAdapter parameterFactory, ModelAdapter modelFactory, ICollection<ApiDescription> apiDescriptions)
        {
            _apiDescriptions = apiDescriptions;
            _modelFactory = modelFactory;
            _parameterFactory = parameterFactory;
            _appVirtualPath = virtualPath.TrimEnd('/');
            _docProvider = docProvider;
        }

        #endregion --- fields & ctors ---



        public ApiDeclaration CreateApiDeclaration(string root, string controllerName)
        {
            var apiDescriptions = GetApiDescriptions(controllerName);

            var apis = this.CreateApi(apiDescriptions);
            var models = _modelFactory.GetModels(apiDescriptions);
            var apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();

            var delcaration = new ApiDeclaration()
            {
                apiVersion = apiVersion,
                swaggerVersion = G.SWAGGER_VERSION,
                basePath = root + _appVirtualPath,
                resourcePath = controllerName,
                apis = apis.ToList(),
                models = models
            };

            return delcaration;
        }


        public IEnumerable<Api> CreateApi(IEnumerable<ApiDescription> apiDescs)
        {
            var rtnApis = from apiDescription in apiDescs
                          let operations = CreateOperation(apiDescription)
                          select new Api()
                                     {
                                         path = "/" + apiDescription.RelativePath,
                                         description = apiDescription.Documentation,
                                         operations = operations
                                     };

            return rtnApis;
        }


        public IList<ApiOperation> CreateOperation(ApiDescription apiDesc)
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

            return new List<ApiOperation>() { rApiOperation };
        }

        private static string CalculateResponseClass(Type type)
        {
            string className;
            if (type == null)
            {
                className = "void";
            }
            else if (type.IsGenericType)
            {
                className = type.GetGenericArguments().First().Name;
            }
            else
            {
                className = type.Name;
            }


            return className;
        }

        private IEnumerable<ApiDescription> GetApiDescriptions(string controllerName)
        {
            var filteredDescs = _apiDescriptions
                .Where(d => d.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper() == controllerName.ToUpper())           // current controller
                .Where(d => !(d.Route.Defaults.ContainsKey(G.SWAGGER)));                                        // and not swagger doc meta route '/api/docs/...'

            return filteredDescs;
        }
    }
}
