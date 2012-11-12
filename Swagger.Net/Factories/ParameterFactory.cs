using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    public class ParameterFactory
    {
        #region --- fields & ctors ---

        private readonly XmlCommentDocumentationProvider _docProvider;

        public ParameterFactory(XmlCommentDocumentationProvider docProvider)
        {
            _docProvider = docProvider;
        }

        public ParameterFactory()
        {
            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
        }

        #endregion --- fields & ctors ---

        public List<dynamic> CreateParameters(Collection<ApiParameterDescription> httpParams, string relativePath)
        {
            var rtn = httpParams.Select(p => CreateParameter(p, relativePath));
            return rtn.ToList();
        }

        public dynamic CreateParameter(ApiParameterDescription parameterDescription, string relativePath)
        {
            var paramType = GetParamType(parameterDescription, relativePath);
            var isRequired = !parameterDescription.ParameterDescriptor.IsOptional;
            var dataType = ModelFactory.GetDataType(parameterDescription.ParameterDescriptor.ParameterType).Name;
            var allMany = GetAllowMuliple(parameterDescription.ParameterDescriptor.ParameterType);

            dynamic rtn = new ExpandoObject();
            rtn.name = parameterDescription.Name;
            rtn.dataType = dataType;
            rtn.required = isRequired;
            rtn.description = parameterDescription.Documentation;
            rtn.paramType = paramType;
            rtn.allowMultiple = allMany;
            // allowableValues
        
            return rtn;
        }

        public bool GetAllowMuliple(Type parameterType)
        {
            return parameterType.IsArray || parameterType.GetInterfaces().Any(i => i.Name.Contains("IEnum"));
        }

        private static string GetParamType(ApiParameterDescription parameterDescription, string relativePath)
        {
            var paramType = G.BODY;
            if (parameterDescription.Source == ApiParameterSource.FromUri)
            {
                paramType = relativePath.IndexOf("{" + parameterDescription.Name + "}") > -1 ? G.PATH : G.QUERY;
            }
            return paramType;
        }
    }
}