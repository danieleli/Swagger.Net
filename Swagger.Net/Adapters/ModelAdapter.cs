using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    public class ModelAdapter
    {
        #region --- fields & ctors ---

        private readonly XmlCommentDocumentationProvider _docProvider;

        public ModelAdapter()
        {
            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
        }

        public ModelAdapter(XmlCommentDocumentationProvider documentationProvider)
        {
            _docProvider = documentationProvider;
        }

        #endregion --- fields & ctors ---

        public Dictionary<string,object> GetModels(IEnumerable<ApiDescription> apiDescs)
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

            var rtn = new Dictionary<string,object>();
            foreach (var m in rtnModels)
            {
                dynamic props = new Dictionary<string, object>();
                foreach (var p in m.Value.properties)
                {
                    var type = new {p.type};
                    props[p.Name] = new { id=p.Name, type};
                }
                rtn.Add(m.Key.Name, new {
                                     id = m.Key.Name,
                                     properties = props
                                 });
            }
            return rtn;
        }

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
                    var model = _docProvider.GetApiModel(myType);
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
   
    }
}


