using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
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

        public Dictionary<string, object> GetModels(IEnumerable<ApiDescription> apiDescs)
        {
            var tempDict = new Dictionary<String, object>();
            var types = apiDescs.Select(a => a.ActionDescriptor.ReturnType).ToList();

            var paramTypes = apiDescs.SelectMany(
                a => a.ParameterDescriptions.Select(
                    p => p.ParameterDescriptor.ParameterType));

            types.AddRange(paramTypes);

            var uniqueNonPrimatives = types.Distinct().Where(t => t!=null && !t.IsPrimitive);

            foreach (var uniqueType in uniqueNonPrimatives)
            {
                var adaptation = Adapt(uniqueType.Name, uniqueType.GetProperties());
                tempDict.Add(uniqueType.Name, new { id = uniqueType.Name, properties = adaptation });
            }


            return tempDict;
        }

        public object Adapt(String typeName, PropertyInfo[] props)
        {


            var rtn = new Dictionary<string, object>();
            foreach (var p in props)
            {
                rtn[p.Name] = new { type = p.PropertyType.Name };
            }
            return rtn;

        }


    }
}


   ////"models":{
   ////   "Category":{
   ////      "id":"Category",
   ////      "properties":{
   ////         "id":{
   ////            "type":"long"
   ////         },
   ////         "name":{
   ////            "type":"string"
   ////         }
   ////      }
   ////   },
   ////   "Pet":{
   ////      "id":"Pet",
   ////      "properties":{
   ////         "tags":{
   ////            "items":{
   ////               "$ref":"Tag"
   ////            },
   ////            "type":"Array"
   ////         },
   ////         "id":{
   ////            "type":"long"
   ////         },
   ////         "category":{
   ////            "type":"Category"
   ////         },
   ////         "status":{
   ////            "allowableValues":{
   ////               "valueType":"LIST",
   ////               "values":[
   ////                  "available",
   ////                  "pending",
   ////                  "sold"
   ////               ],
   ////               "valueType":"LIST"
   ////            },
   ////            "description":"pet status in the store",
   ////            "type":"string"
   ////         },
   ////         "name":{
   ////            "type":"string"
   ////         },
   ////         "photoUrls":{
   ////            "items":{
   ////               "type":"string"
   ////            },
   ////            "type":"Array"
   ////         }
   ////      }
   ////   },
   ////   "Tag":{
   ////      "id":"Tag",
   ////      "properties":{
   ////         "id":{
   ////            "type":"long"
   ////         },
   ////         "name":{
   ////            "type":"string"
   ////         }
   ////      }
   ////   }