using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Http.Description;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    public class ParameterMetadataFactory
    {
        private readonly XmlCommentDocumentationProvider _docProvider;

        public ParameterMetadataFactory(XmlCommentDocumentationProvider docProvider)
        {
            _docProvider = docProvider;
        }

        public ParameterMetadataFactory()
        {
            
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
            var paramType = GetParamType(parameterDescription, relativePath);
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

        private static string GetParamType(ApiParameterDescription parameterDescription, string relativePath)
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
            return paramType;
        }
        
    }
}