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

            var uniqueNonPrimatives = types.Where(t => t != null && !t.IsPrimitive).Distinct();
            var trueTypes = uniqueNonPrimatives.Select(GetDataType);
            var refilteredTypes = trueTypes.Where(t => t != null && !t.IsPrimitive).Distinct();

            foreach (var uniqueType in refilteredTypes)
            {
                var modelItem = CreateModel(uniqueType);
                if (tempDict.ContainsKey(uniqueType.Name))
                {
                    //skip
                }
                else
                {
                    tempDict.Add(uniqueType.Name, modelItem);
                }
            }
            return tempDict;
        }

        public object CreateModel(Type type)
        {
            var modelProperties = new Dictionary<string, object>();
            foreach (var prop in type.GetProperties())
            {
                object item;
                if (prop.PropertyType.IsArray)
                {

                    
                    item = new
                    {
                        type = "Array",
                       items = new {Sref=prop.PropertyType.GetElementType().Name}
                    };

                    

                }
                else if (prop.PropertyType.IsPrimitive)
                {
                    item = new
                   {
                       type = prop.PropertyType.Name
                   };
                }
                else
                {
                    var itemDocs = _docProvider.GetDocumentation(prop.PropertyType);
                    if (itemDocs.StartsWith("No"))
                    {
                        item = new
                        {
                            type = prop.PropertyType.Name
                        };
                    }
                    else
                    {
                        item = new
                        {
                            type = prop.PropertyType.Name,
                            description = itemDocs
                        };
                    }

                }
                modelProperties[prop.Name] = item;


            }

            return new
                       {
                           id = type.Name,
                           properties = modelProperties,
                           description = _docProvider.GetDocumentation(type)
                       };


        }

        public static Type GetDataType(Type inputType)
        {
            if (inputType.IsPrimitive || inputType == typeof(string)) return inputType;

            if (inputType.IsArray)
            {
                return inputType.GetElementType();
            }

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