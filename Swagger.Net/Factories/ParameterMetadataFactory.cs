using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    public class ParameterMetadataFactory
    {
        #region --- fields & ctors ---

        private readonly XmlCommentDocumentationProvider _docProvider;

        public ParameterMetadataFactory(XmlCommentDocumentationProvider docProvider)
        {
            _docProvider = docProvider;
        }

        public ParameterMetadataFactory()
        {
            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
        }

        #endregion --- fields & ctors ---

        public IList<Parameter> CreateParameters(Collection<ApiParameterDescription> httpParams, string relativePath)
        {
            var rtn = httpParams.Select(p => CreateParameter(p, relativePath));
            return rtn.ToList();
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