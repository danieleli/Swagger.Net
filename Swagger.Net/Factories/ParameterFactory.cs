using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public List<ApiParameter> CreateParameters(Collection<ApiParameterDescription> httpParams, string relativePath)
        {
            var rtn = httpParams.Select(p => CreateParameter(p, relativePath));
            return rtn.ToList();
        }

        public ApiParameter CreateParameter(ApiParameterDescription parameterDescription, string relativePath)
        {
            var trueParamType = parameterDescription.ParameterDescriptor.ParameterType;

            var paramType = GetParamType(parameterDescription, relativePath);
            var isRequired = !parameterDescription.ParameterDescriptor.IsOptional;
            var dataType =  GetFriendlyTypeName(trueParamType);
            var allowMuliple = GetAllowMuliple(trueParamType);

            if (parameterDescription.ParameterDescriptor.ParameterType.IsEnum)
            {
                var possibleValues = new AllowableValues(trueParamType);
                return new ApiEnumParameter()
                {
                    name = parameterDescription.Name,
                    dataType = dataType,
                    paramType = paramType.ToString(),
                    description = parameterDescription.Documentation,
                    allowMultiple = allowMuliple,
                    required = isRequired,
                    allowableValues = possibleValues
                };        
            }

            return  new ApiParameter()
                          {
                              name = parameterDescription.Name,
                              dataType = dataType,
                              paramType = paramType.ToString(),
                              description = parameterDescription.Documentation,
                              allowMultiple = allowMuliple,
                              required = isRequired
                          };
        }

        public bool GetAllowMuliple(Type parameterType)
        {
            return parameterType.IsArray || parameterType.GetInterfaces().Any(i => i.Name.Contains("IEnum"));
        }

        private static ParamType GetParamType(ApiParameterDescription parameterDescription, string relativePath)
        {
            var paramType = ParamType.body;
            if (parameterDescription.Source == ApiParameterSource.FromUri)
            {
                paramType = relativePath.IndexOf("{" + parameterDescription.Name + "}") > -1 && !parameterDescription.ParameterDescriptor.IsOptional ? ParamType.path : ParamType.query;
            }
            return paramType;
        }

        private string GetFriendlyTypeName(Type type)
        {
            if (type.IsArray)
            {   // Array
                return type.GetElementType().Name + "[]";
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var gType = type.GetGenericArguments().First();
                var name = Utils.GetCleanTypeName(gType);
                name = name.Substring(name.IndexOf(".") + 1);
                return  name + "?";
            }
            return type.Name;
        }
    }
}