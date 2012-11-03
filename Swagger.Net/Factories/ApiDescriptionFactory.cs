﻿using System;
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
    public interface IApiDescriptionFactory
    {
        ResourceDescription CreateApiDescription(Uri uri, string controllerName);
        IList<Api> CreateApiElements( string controllerName, IEnumerable<ApiDescription> descriptions);
        IList<Operation> CreateOperations(ApiDescription desc, XmlCommentDocumentationProvider docProvider);
        IList<Parameter> CreateParameters(Collection<HttpParameterDescriptor> httpParams);
    }

    public class ApiDescriptionFactory : IApiDescriptionFactory
    {

        private readonly string _appVirtualPath;
        public ApiDescriptionFactory() : this(HttpRuntime.AppDomainAppVirtualPath) { }
        public ApiDescriptionFactory(string appVirtualPath)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/');
        }

        public ResourceDescription CreateApiDescription(Uri uri, string controllerName)
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

        public IList<Api> CreateApiElements( string controllerName, IEnumerable<ApiDescription> descriptions)
        {
            var rtnApis = new List<Api>();

            foreach (var desc in descriptions)
            {
                if (desc.Route.Defaults.ContainsKey(SwaggerConstants.SWAGGER)) continue;

                var apiControllerName = desc.ActionDescriptor.ControllerDescriptor.ControllerName;
                if (apiControllerName.ToUpper() == SwaggerConstants.SWAGGER.ToUpper()) continue;

                // Make sure we only report the current controller docs
                if (!apiControllerName.Equals(controllerName)) continue;

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

            return new List<Operation>(){rApiOperation};  
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
