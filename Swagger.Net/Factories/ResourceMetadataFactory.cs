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
    public class ResourceMetadataFactory
    {

        #region --- fields & ctors ---

        private readonly string _appVirtualPath;
        private readonly XmlCommentDocumentationProvider _docProvider;
        private readonly ParameterMetadataFactory _parameterFactory;
        private readonly ICollection<ApiDescription> _apiDescriptions;
        private readonly ModelMetadataFactory _modelFactory;

        public ResourceMetadataFactory()
        {
            _appVirtualPath = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/'); ;
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
            _parameterFactory = new ParameterMetadataFactory();

            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
            _modelFactory = new ModelMetadataFactory(_docProvider);
        }

        public ResourceMetadataFactory(string virtualPath, XmlCommentDocumentationProvider docProvider, ParameterMetadataFactory parameterFactory, ModelMetadataFactory modelFactory,  ICollection<ApiDescription> apiDescriptions)
        {
            _apiDescriptions = apiDescriptions;
            _modelFactory = modelFactory;
            _parameterFactory = parameterFactory;
            _appVirtualPath = virtualPath.TrimEnd('/');
            _docProvider = docProvider;
        }

        #endregion --- fields & ctors ---

        private IEnumerable<ApiDescription> FilterApis(string controllerName)
        {
            var filteredDescs = _apiDescriptions
                .Where(d => d.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper() == controllerName.ToUpper())           // current controller
                .Where(d => !(d.Route.Defaults.ContainsKey(G.SWAGGER)));                                        // and not swagger doc meta route '/api/docs/...'

            return filteredDescs;
        }

        public ResourceDescription GetDocs(string root, string controllerName)
        {

            var docs = this.CreateResourceMetadata(root, controllerName);
            var filteredApiDescs = FilterApis(controllerName);
            var apis = this.CreateApiElements(filteredApiDescs);
            docs.apis.AddRange(apis);

            var models = _modelFactory.GetModels(filteredApiDescs);

            docs.models.AddRange(models);
            return docs;
        }

        public ResourceDescription CreateResourceMetadata(string rootUrl, string controllerName)
        {

            var rtnResource = new ResourceDescription()
            {
                apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString(),
                swaggerVersion = G.SWAGGER_VERSION,
                basePath = rootUrl + _appVirtualPath,
                resourcePath = controllerName
            };

            return rtnResource;
        }

        public IList<Api> CreateApiElements(IEnumerable<ApiDescription> apiDescs)
        {
            var apis = apiDescs.Select(GetApiMetadata);
            return apis.ToList();
        }

        /// <summary>
        /// Create ApiRoot
        ///     Add Operations
        ///         Add Parameters
        /// </summary>
        private Api GetApiMetadata(ApiDescription apiDesc)
        {
            var api = CreateApiRoot(apiDesc);

            var operations = CreateOperationRoot(apiDesc);
            foreach (var op in operations)
            {
                var parameters = _parameterFactory.CreateParameters(apiDesc.ParameterDescriptions, apiDesc.RelativePath);
                op.parameters.AddRange(parameters);
            }
            api.operations.AddRange(operations);

            return api;
        }

        public Api CreateApiRoot(ApiDescription desc)
        {
            var api = new Api()
            {
                path = "/" + desc.RelativePath,
                description = desc.Documentation
            };

            return api;
        }

        public IList<Operation> CreateOperationRoot(ApiDescription apiDesc)
        {

            var responseClass = CalculateResponseClass(apiDesc.ActionDescriptor.ReturnType);
            var remarks = _docProvider.GetRemarks(apiDesc.ActionDescriptor);

            var rApiOperation = new Operation()
            {
                httpMethod = apiDesc.HttpMethod.ToString(),
                nickname = apiDesc.ActionDescriptor.ActionName,
                responseClass = responseClass,
                summary = apiDesc.Documentation,
                notes = remarks,
            };

            return new List<Operation>() { rApiOperation };
        }

        private static string CalculateResponseClass(Type returnType)
        {
            return returnType == null ? "void" : returnType.Name;
        }
        
    }
}
