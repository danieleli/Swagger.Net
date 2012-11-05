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
    public class ResourceMetadataFactory
    {

        #region --- fields & ctors ---

        private string _appVirtualPath;
        private XmlCommentDocumentationProvider _docProvider;
        private readonly ParameterMetadataFactory _parameterFactory;
        private readonly ICollection<ApiDescription> _apiDescriptions;

        public ResourceMetadataFactory(Collection<ApiDescription> apiDescriptions)
        {
            _apiDescriptions = apiDescriptions;
            var path = HttpRuntime.AppDomainAppVirtualPath;
            var docProvider = (IDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
            _parameterFactory = new ParameterMetadataFactory();
            initialize(path, docProvider);
        }

        public ResourceMetadataFactory(string virtualPath, XmlCommentDocumentationProvider docProvider, ParameterMetadataFactory parameterFactory)
        {
            _parameterFactory = parameterFactory;
            initialize(virtualPath, docProvider);
        }

        public void initialize(string appVirtualPath, IDocumentationProvider docProvider)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/');
            _docProvider = (XmlCommentDocumentationProvider)docProvider;
        }

        #endregion --- fields & ctors ---

        private void AddIfValid(Type myType, Dictionary<Type, Model> rtnModels)
        {
            if (IsOfInterest(myType))
            {
                if (myType.IsGenericType)
                {
                    myType = myType.GetGenericArguments()[0];
                }
                if (!rtnModels.ContainsKey(myType))
                {
                    var model = this.GetResourceModel(myType);
                    rtnModels.Add(myType, model);
                }
            }
        }

        private bool IsOfInterest(Type returnType)
        {
            if (returnType == null) return false;

            if (returnType.IsGenericType)
            {
                returnType = returnType.GetGenericArguments()[0];
            }

            if (returnType.IsPrimitive || returnType == typeof(string))
            {
                return false;
            }
            return true;
        }


        public IEnumerable<Model> GetModels(IEnumerable<ApiDescription> apiDescs)
        {
            var rtnModels = new Dictionary<Type, Model>();
            foreach (var apiDesc in apiDescs)
            {
                AddIfValid(apiDesc.ActionDescriptor.ReturnType, rtnModels);

                foreach (var param in apiDesc.ParameterDescriptions)
                {
                    AddIfValid(param.ParameterDescriptor.ParameterType, rtnModels);
                }
            }

            return rtnModels.Values;
        }
        private IEnumerable<ApiDescription> FilterApis(string controllerName)
        {
            var filteredDescs = _apiDescriptions
                .Where(d => d.ActionDescriptor.ControllerDescriptor.ControllerName == controllerName)           // current controller
                .Where(d => !(d.Route.Defaults.ContainsKey(G.SWAGGER)));                                        // and not swagger doc meta route '/api/docs/...'

            return filteredDescs;
        }

        public ResourceDescription GetDocs(string root, string controllerName)
        {

            var docs = this.CreateResourceMetadata(root, controllerName);
            var filteredApiDescs = FilterApis(controllerName);
            var apis = this.CreateApiElements(filteredApiDescs);
            docs.apis.AddRange(apis);

            var models = GetModels(filteredApiDescs);

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
            var apis = apiDescs.Select(apiDesc => GetApiMetadata(apiDesc));
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


        public Model GetResourceModel(Type type)
        {
            return _docProvider.GetApiModel(type);
        }

        public IEnumerable<Model> GetResourceModels(IEnumerable<Type> paramTypes)
        {
            return paramTypes.Select(GetResourceModel);   // Function pointer
        }

    }
}
