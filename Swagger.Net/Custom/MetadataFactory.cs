using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Swagger.Net.Custom.Extensions;

namespace Swagger.Net.Custom
{
    public class MetadataFactory
    {

        private readonly IList<ApiDescription> _apis;
        private readonly string[] _distinctControllerNames;

        public MetadataFactory(IList<ApiDescription> apis)
        {
            _apis = apis;

            _distinctControllerNames = apis
                .Select(a => a.ActionDescriptor.ControllerDescriptor.ControllerName)
                .Distinct()
                .OrderBy(s => s)
                .ToArray();
        }

        public IEnumerable<ControllerMetadata> CreateMetadata()
        {
            var rootControllers = RootControllerFinder.GetRootControllers(_distinctControllerNames);

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

            var controllerDescriptor = currentApiDescs.First().ActionDescriptor.ControllerDescriptor;
            
            var controllerMeta = controllerDescriptor.GetDocs(parentControllerName);
            controllerMeta.ModelType = GetModelMeta(currentApiDescs);
            controllerMeta.Operations = currentApiDescs.Select(api => api.ActionDescriptor.GetDocs(parentControllerName, api.RelativePath, api.HttpMethod));
            controllerMeta.Children = GetChildren(controllerName, _distinctControllerNames);

            return controllerMeta;
        }

        private static TypeMetadata GetModelMeta(IEnumerable<ApiDescription> currentApiDescs)
        {
            var possibleModelTypes = GetPossibleModelTypes(currentApiDescs);
            var modelType = GetModelType(possibleModelTypes);
            var modelMeta = modelType == null ? null : modelType.GetDocs();
            return modelMeta;
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



        #region "Model 'n Type"

        private static IEnumerable<Type> GetPossibleModelTypes(IEnumerable<ApiDescription> currentApiDescs)
        {
            var returnTypes = currentApiDescs.Where(a => a.HttpMethod == HttpMethod.Get).Select(a=>a.ActionDescriptor.ReturnType);
            return returnTypes;
        }

        private static Type GetModelType(IEnumerable<Type> possibleTypes)
        {
            Type rtn = null;
            foreach (var t in possibleTypes)
            {

                // todo: move this into config file
                if (t.FullName.StartsWith("MXM.API.Services.Models"))
                {
                    if (t.IsGenericType)
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

        #endregion


    }
}