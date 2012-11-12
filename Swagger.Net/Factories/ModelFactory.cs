using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    public class ModelFactory
    {
        #region --- fields & ctors ---

        private readonly XmlCommentDocumentationProvider _docProvider;

        public ModelFactory()
        {
            _docProvider = (XmlCommentDocumentationProvider)GlobalConfiguration.Configuration.Services.GetService((typeof(IDocumentationProvider)));
        }

        public ModelFactory(XmlCommentDocumentationProvider documentationProvider)
        {
            _docProvider = documentationProvider;
        }

        #endregion --- fields & ctors ---

        public Dictionary<string, object> GetModels(IEnumerable<ApiDescription> apiDescs)
        {
            var types = GetUniqueTypes(apiDescs);

            var d = new Dictionary<String, object>();
            foreach (var t in types)
            {
                var modelItem = CreateModel(t);
                if (!d.ContainsKey(t.Name))
                {
                    d.Add(t.Name, modelItem);
                }
            }
            return d;
        }

        private static IEnumerable<Type> GetUniqueTypes(IEnumerable<ApiDescription> apiDescs)
        {
            var types = apiDescs.Select(a => a.ActionDescriptor.ReturnType).ToList();

            var paramTypes = apiDescs.SelectMany(
                a => a.ParameterDescriptions.Select(
                    p => p.ParameterDescriptor.ParameterType));

            types.AddRange(paramTypes);

            var uniqueNonPrimatives = types.Where(t => t != null && !t.IsPrimitive).Distinct();
            var trueTypes = uniqueNonPrimatives.Select(GetDataType);
            var refilteredTypes = trueTypes.Where(t => t != null && !t.IsPrimitive).Distinct();

            return refilteredTypes;
        }

        public object CreateModel(Type type)
        {
            var properties = new Dictionary<string, object>();
            foreach (var prop in type.GetProperties())
            {
                properties[prop.Name] = GetProperty(prop);
            }

            return new {
                           id = type.Name,
                           properties = properties,
                           description = _docProvider.GetDocumentation(type)
                       };
        }

        private object GetProperty(PropertyInfo prop)
        {
            
            object item;
            if (prop.PropertyType.IsArray)
            {   // Array
                item = new {type = "Array", items = new {Sref = prop.PropertyType.GetElementType().Name}};
            }
            else if (prop.PropertyType.IsPrimitive)
            {   // Primative
                item = new {type = prop.PropertyType.Name};
            }
            else
            {
                var itemDocs = _docProvider.GetDocumentation(prop.PropertyType);
                if (itemDocs.StartsWith("No"))
                {   // No Documentation
                    item = new {type = prop.PropertyType.Name};
                }
                else
                {
                    item = new {type = prop.PropertyType.Name, description = itemDocs};
                }
            }

            return item;
        }

        public static Type GetDataType(Type inputType)
        {   // Primative
            if (inputType.IsPrimitive || inputType == typeof(string)) return inputType;

            // Array
            if (inputType.IsArray) { return inputType.GetElementType(); }

            // IEnumerable
            if (inputType.GetInterfaces().Any(i => i.Name.Contains("IEnum")))
            {
                if (inputType.IsGenericType)
                {
                    return inputType.GetGenericArguments().First();
                }
                return inputType.GetElementType();
            }

            return inputType;
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