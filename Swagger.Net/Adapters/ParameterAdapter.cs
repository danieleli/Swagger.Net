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
    public class ParameterAdapter
    {
        #region --- fields & ctors ---

        private readonly XmlCommentDocumentationProvider _docProvider;

        public ParameterAdapter(XmlCommentDocumentationProvider docProvider)
        {
            _docProvider = docProvider;
        }

        public ParameterAdapter()
        {
            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
        }

        #endregion --- fields & ctors ---

        public IList<dynamic> CreateParameters(Collection<ApiParameterDescription> httpParams, string relativePath)
        {
            var rtn = httpParams.Select(p => CreateParameter(p, relativePath));
            return rtn.ToList();
        }

        public dynamic CreateParameter(ApiParameterDescription parameterDescription, string relativePath)
        {
            var paramType = GetParamType(parameterDescription, relativePath);
            var isRequired = !parameterDescription.ParameterDescriptor.IsOptional;
            var dataType = ModelAdapter.GetDataType(parameterDescription.ParameterDescriptor.ParameterType).Name;
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
            if (parameterType.IsArray || parameterType.GetInterfaces().Any(i => i.Name.Contains("IEnum")))
            {
                return true;
            }
            return false;
        }

        private static string GetParamType(ApiParameterDescription parameterDescription, string relativePath)
        {
            var paramType = G.BODY;
            if (parameterDescription.Source == ApiParameterSource.FromUri)
            {
                if (relativePath.IndexOf("{" + parameterDescription.Name + "}") > -1)
                {
                    paramType = G.PATH;
                }
                else
                {
                    paramType = G.QUERY;
                }
            }
            return paramType;
        }
        
    }
}