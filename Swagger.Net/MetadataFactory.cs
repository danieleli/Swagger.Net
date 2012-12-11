using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Swagger.Net
{
    /// <summary>
    /// This is a sample class
    /// </summary>
    /// <remarks>Use this class as a sample in the documentation</remarks>
    public class Foo
    {
        /// <summary>
        /// This is integer id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of foo
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The city in which foo can be found
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// How much foo charges
        /// </summary>
        public double Amount { get; set; }
    }

    public abstract class Metadata
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
    }

    public class ControllerMetadata : Metadata
    {
        public string ParentController { get; set; }
        public string Controller { get; set; }
        public IEnumerable<OperationMetadata> Operations { get; set; }
        public IEnumerable<ControllerMetadata> Children { get; set; }
        public TypeMetadata ModelType { get; set; }
    }

    public class OperationMetadata : Metadata
    {
        public string ReturnsComment { get; set; }
        public TypeMetadata ReturnType { get; set; }
        public string RelativePath { get; set; }
        public string AlternatePath { get; set; }
        public string HttpMethod { get; set; }
        public IEnumerable<ErrorMetadata> ErrorResponses { get; set; }
        public IEnumerable<ParamMetadata> Params { get; set; }
    }

    public class ParamMetadata
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public TypeMetadata Type { get; set; }
    }

    public class TypeMetadata : Metadata
    {    
        public IEnumerable<PropertyMetadata> Properties { get; set; }
    }

    public class PropertyMetadata : Metadata
    {
        public string DataType { get; set; }
        public string Value { get; set; }
    }

    public class ModelMetadata : TypeMetadata
    {
        public ModelMetadata()
        {
            this.Samples = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Samples { get; set; }

    }

    public class ErrorMetadata
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class MetadataFactory
    {
        const string IPSUM = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi. Nam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer possim assum. Typi non habent claritatem insitam; est usus legentis in iis qui facit eorum claritatem. Investigationes demonstraverunt lectores legere me lius quod ";

        private readonly IList<ApiDescription> _apis;
        private readonly XmlCommentDocumentationProvider _docProvider;
        private readonly string[] _distinctControllerNames;

        public MetadataFactory(IList<ApiDescription> apis, XmlCommentDocumentationProvider docProvider)
        {
            _apis = apis;
            _docProvider = docProvider;

            _distinctControllerNames = apis
                .Select(a => a.ActionDescriptor.ControllerDescriptor.ControllerName)
                .Distinct()
                .OrderBy(s => s)
                .ToArray();
        }

        public IEnumerable<ControllerMetadata> CreateMetadata()
        {
            var rootControllers = RootControllerFilter.GetRootControllers(_distinctControllerNames);

            var rtn = new List<ControllerMetadata>();

            foreach (var currentController in rootControllers)
            {
                var controlMeta = GetControllerMetadata(currentController, null);
                rtn.Add(controlMeta);
            }
            return rtn;
        }

        private ControllerMetadata GetControllerMetadata(string controllerName, string parentControllerName)
        {

            var currentApiDescs = _apis
                .Where(a => a.ActionDescriptor.ControllerDescriptor.ControllerName == controllerName).ToArray();

            var possibleModelTypes = GetPossibleModelTypes(currentApiDescs);
            var modelType = GetModelType(possibleModelTypes) ?? typeof (Foo);
            var modelMeta = GetModelMetaData(modelType);  
            
            

            var operations = GetOperations(currentApiDescs, parentControllerName);
            var children = GetChildren(controllerName, _distinctControllerNames);

            var controlMeta = new ControllerMetadata()
                {
                    Name = controllerName,
                    Summary = _docProvider.GetDocumentation(currentApiDescs.First().ActionDescriptor.ControllerDescriptor.ControllerType),
                    Remarks = IPSUM,
                    Operations = operations,
                    Children = children,
                    Controller = controllerName,
                    ParentController = parentControllerName,
                    ModelType = modelMeta
                };

            return controlMeta;
        }

        private IEnumerable<ControllerMetadata> GetChildren(string parentControllerName, IEnumerable<string> distinctControllerNames)
        {
            var childrenNames = distinctControllerNames
                .Where(name => name.StartsWith(parentControllerName) && name != parentControllerName);

            var rtnList = new List<ControllerMetadata>();
            foreach (var childName in childrenNames)
            {
                // recursive call to GetControllerMetadata
                var ctrlMeta = GetControllerMetadata(childName, parentControllerName);
                rtnList.Add(ctrlMeta);
            }
            
            return rtnList;
        }

        private IEnumerable<OperationMetadata> GetOperations(IEnumerable<ApiDescription> currentApiDescs, string parentControllerName)
        {
            var rtn = new List<OperationMetadata>();
            foreach (var apiDescription in currentApiDescs)
            {
                var paramz = GetParams(apiDescription.ActionDescriptor);
                var returnType = GetTypeMetaData(apiDescription.ActionDescriptor.ReturnType);

                var path = apiDescription.RelativePath.ToLower();
                var altPath = "";
                if (parentControllerName != null)
                {
                    altPath = path;
                    path = GetAlternatePath(parentControllerName,
                                            apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName,
                                            apiDescription.RelativePath).ToLower();
                }

                var op = new OperationMetadata()
                    {
                        Name = apiDescription.ActionDescriptor.ActionName,
                        HttpMethod = apiDescription.HttpMethod.ToString(),
                        RelativePath = path,
                        AlternatePath = altPath,
                        Summary = _docProvider.GetDocumentation(apiDescription.ActionDescriptor),
                        Remarks = _docProvider.GetRemarks(apiDescription.ActionDescriptor),
                        ReturnType = returnType,
                        ReturnsComment = _docProvider.GetResponseClass(apiDescription.ActionDescriptor),
                        Params = paramz,
                        ErrorResponses = null

                    };
                rtn.Add(op);
            }
            return rtn;
        }

        private IEnumerable<ParamMetadata> GetParams(HttpActionDescriptor actionDescriptor)
        {
            var rtn = new List<ParamMetadata>();
            foreach (var parmDescriptor in actionDescriptor.GetParameters())
            {
                var p = new ParamMetadata()
                    {
                        Name = parmDescriptor.ParameterName,
                        Comment = _docProvider.GetDocumentation(parmDescriptor),
                        Type = new TypeMetadata()
                            {
                                Name = parmDescriptor.ParameterType.Name,
                                Properties = null
                            }
                    };

                rtn.Add(p);
            }
            return rtn;
        }

        #region "Model 'n Type"

        private IEnumerable<Type> GetPossibleModelTypes(IEnumerable<ApiDescription> currentApiDescs)
        {
            var returnTypes = currentApiDescs.Where(a => a.HttpMethod == HttpMethod.Get).Select(a=>a.ActionDescriptor.ReturnType);
            return returnTypes;
        }

        private Type GetModelType(IEnumerable<Type> types)
        {
            Type rtn = null;
            foreach (var t in types)
            {
                if (t.FullName.StartsWith("MXM.API.Services.Models"))
                {
                    if (t.FullName.Contains("PagedList"))
                    {
                        rtn = t.GetGenericArguments().First();
                        break;
                    }
                    rtn = t;
                    break;
                }
            }
            return rtn;
        } 

        public ModelMetadata GetModelMetaData(Type type)
        {
            var props = GetPropertyMetadatas(type);
            var samples = GetSamples(type);
            var returnMeta = new ModelMetadata()
            {
                Name = type.Name,
                Summary = _docProvider.GetDocumentation(type),
                Remarks = _docProvider.GetDocumentation(type),
                Properties = props,
                Samples = samples
            };

            return returnMeta;
        }

        private static Dictionary<string, string> GetSamples(Type type)
        {
            var rtn = new Dictionary<string, string>();
            var item = Activator.CreateInstance(type);
            rtn.Add("JSON", JsonConvert.SerializeObject(item));
            var serializer = new XmlSerializer(type);
            var stream = new MemoryStream();
            try
            {
                serializer.Serialize(stream, item);
                rtn.Add("XML", "some xml shit here");
            }
            catch
            {
                // eat it.
            }

            return rtn;
        }

        // todo: test me
        public IEnumerable<PropertyMetadata> GetPropertyMetadatas(Type type)
        {
            var props = new List<PropertyMetadata>();

            foreach (var propertyInfo in type.GetProperties())
            {
                var p = new PropertyMetadata()
                {
                    DataType = propertyInfo.PropertyType.Name,
                    Name = propertyInfo.Name,
                    Summary = _docProvider.GetDocumentation(propertyInfo),
                };
                props.Add(p);
            }
            return props;
        }

        private TypeMetadata GetTypeMetaData(Type type)
        {
            var props = GetPropertyMetadatas(type);

            var returnMeta = new TypeMetadata()
            {
                Name = type.Name,
                Summary = _docProvider.GetDocumentation(type),
                Remarks = _docProvider.GetDocumentation(type),
                Properties = props
            };

            return returnMeta;
        }

        #endregion


        private static string GetAlternatePath(string parentControllerName, string currentControllerName, string relativePath)
        {
            var path = "/" + relativePath;

            if (currentControllerName != parentControllerName && relativePath.Contains(currentControllerName))
            {
                var shortName = currentControllerName.Substring(parentControllerName.Length);
                path = "/" + parentControllerName + "/{id}/" + shortName;


                var morePath = relativePath.Substring(currentControllerName.Length);
                if (morePath.Contains("/{id}") && morePath.ToLower().Contains("subid={subid}"))
                {
                    morePath = morePath.Replace("/{id}", "/{subId}");
                    morePath = morePath.ToLower().Replace("subid={subid}", "");
                }
                else
                {
                    morePath = morePath.Replace("/{id}", "");
                }

                if (morePath.IndexOf("?") > 0 && morePath.IndexOf("?") == morePath.Length - 1)
                {
                    morePath = morePath.Substring(0, morePath.Length - 1);
                }

                path += morePath;
            }
            return path;
        }
    }
}