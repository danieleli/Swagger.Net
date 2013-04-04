using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
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
            var types = GetAllTypes(apiDescs);

            var uniqueNonPrimatives = types.Where(t =>
                    t != null &&
                    !t.IsPrimitive &&
                    !(t == typeof(String)) &&
                    !(t == typeof(Guid))
                    ).Distinct();
            var trueTypes = uniqueNonPrimatives.Select(GetDataType);
            var refilteredTypes = trueTypes.Where(t => t != null &&
                    !t.IsPrimitive &&
                    !(t == typeof(String)) &&
                    !(t == typeof(Guid))
                    ).Distinct();

            return refilteredTypes;
        }

        private static IEnumerable<Type> GetAllTypes(IEnumerable<ApiDescription> apiDescs)
        {
            // return types
            // except httpresponsemessages
            var types = apiDescs
                .Where(t => !(t.ActionDescriptor.ReturnType == typeof(System.Net.Http.HttpResponseMessage)) && !t.ActionDescriptor.ReturnType.IsInstanceOfType(typeof(System.Net.Http.HttpResponseMessage))
                )
                .Select(a => a.ActionDescriptor.ReturnType).ToList();

            // param types
            var paramTypes = apiDescs.SelectMany(
                a => a.ParameterDescriptions.Select(
                    p => p.ParameterDescriptor.ParameterType));
            
            // all types
            types.AddRange(paramTypes);

            //inner types non-system, single element or collection
            var propertiesTypes = types
                .Where(tn => !tn.Namespace.StartsWith("System"))
                .SelectMany(
                t => t.GetProperties()
                    .Where(n => !GetDataType(n.PropertyType).Namespace.StartsWith("System"))
                    .Select(p => GetDataType(p.PropertyType))).ToList();

            types.AddRange(propertiesTypes);
            return types;
        }

        public object CreateModel(Type type)
        {
            var properties = new Dictionary<string, object>();


            if (type.IsEnum)
            {
                var allowableVals = new AllowableValues(type);
                var itemDocs = _docProvider.GetDocumentation(type);
                var item = new { allowableValues = allowableVals, description = itemDocs, type = "string" };
                properties["PossibleValues"] = item;
            }
            else
            {
                foreach (var prop in type.GetProperties())
                {
                    properties[prop.Name] = GetProperty(prop);
                }
            }

            return new ApiModel
            {
                id = type.Name,
                properties = properties,
                description = _docProvider.GetDocumentation(type)
            };
        }

        private object GetProperty(PropertyInfo prop)
        {

            object item;
            var docs = _docProvider.GetDocumentation(prop);
            if (prop.PropertyType.IsArray || prop.PropertyType.IsGenericType)
            {   // Array Or Collection
                var elemType = GetDataType(prop.PropertyType);
                item = new
                {
                    type = elemType.Name + "[]",
                    items = new { Sref = elemType.Name },
                    description = docs
                };
            }
            else if (prop.PropertyType.IsPrimitive)
            {   // Primative
                item = new
                {
                    type = prop.PropertyType.Name,
                    description = docs
                };
            }
            else if (prop.PropertyType.IsEnum)
            {
                //"status": {
                //  "allowableValues": {
                //      "valueType": "LIST",
                //      "values": [
                //   "available",
                //   "pending",
                //   "sold"
                //],
                //      "valueType": "LIST"
                //  },
                //  "description": "pet status in the store",
                //  "type": "string"
                //}
                // Enum
                var allowableVals = new AllowableValues(prop.PropertyType);
                item = new { allowableValues = allowableVals, description = docs, type = "string" };
            }

            //else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
            else if (prop.PropertyType.IsGenericType)
            {
                var gType = prop.PropertyType.GetGenericArguments().First();
                var name = Utils.GetCleanTypeName(gType);
                name = name.Substring(name.IndexOf(".") + 1);
                item = new { type = name + "?", description = docs };
            }
            else
            {

                item = new { type = prop.PropertyType.Name, description = docs };

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
                    return GetDataType(inputType.GetGenericArguments().First());
                }
                return inputType.GetElementType();
            }

            if (inputType.IsGenericType)
            {
                return GetDataType(inputType.GetGenericArguments().First());
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