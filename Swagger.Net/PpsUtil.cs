using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Http.Description;
using Swagger.Net.Models;

namespace Swagger.Net
{
    public static class PpsUtil
    {

        //public static IEnumerable<ControllerMetadata> CreateMetadata(IList<ApiDescription> allApiDescriptions)
        //{
        //    var distinctControllers = allApiDescriptions
        //        .Select(a => a.ActionDescriptor.ControllerDescriptor.ControllerName)
        //        .Distinct()
        //        .OrderBy(s => s)
        //        .ToList();
            
        //    var rootControllers = GetRootControllers(distinctControllers);

        //    var rtn = new List<ControllerMetadata>();
            
        //    foreach (var currentController in rootControllers)
        //    {
        //        var controlMeta = GetControllerMetadata(allApiDescriptions, currentController);
        //        rtn.Add(controlMeta);
        //    }
        //    return rtn;
        //}

        //private static ControllerMetadata GetControllerMetadata(IEnumerable<ApiDescription> allApiDescriptions, string controllerName)
        //{

        //    var currentApiDescs = allApiDescriptions
        //        .Where(a => a.ActionDescriptor.ControllerDescriptor.ControllerName == controllerName);

        //    var operations = GetOperations(currentApiDescs);
  
        //    var controlMeta = new ControllerMetadata()
        //        {
        //            Name = controllerName,
        //            Summary = "Summary",
        //            Remarks = "Remarks",
        //            Children = null,
        //            Operations = operations,
        //            Controller = null
        //        };
        //    return controlMeta;
        //}

        //private static IEnumerable<OperationMetadata> GetOperations(IEnumerable<ApiDescription> currentApiDescs)
        //{
        //    var rtn = new List<OperationMetadata>();
        //    foreach (var apiDescription in currentApiDescs)
        //    {
        //        var paramz = GetParams(apiDescription.ActionDescriptor);
        //        var rType = apiDescription.ActionDescriptor.ReturnType;

        //        var returnMeta = new ModelType()
        //            {
        //                Name = rType.Name,
        //                Summary = "Summary",
        //                Remarks = "Remarks",
        //                Properties = null
        //            };
                
        //        var op = new OperationMetadata()
        //            {
        //                Name = apiDescription.ActionDescriptor.ActionName,
        //                Summary = "Summary",
        //                Remarks = "Remarks",
        //                RelativePath = apiDescription.RelativePath,
        //                ReturnType = returnMeta,
        //                ReturnsComment = "ReturnsComment",
        //                Params = paramz,
        //                ErrorResponses = null

        //            };
        //        rtn.Add(op);
        //    }
        //    return rtn;
        //}

        //private static IEnumerable<ParamMetadata> GetParams(HttpActionDescriptor actionDescriptor)
        //{
        //    var rtn = new List<ParamMetadata>();
        //    foreach (var parmDescriptor in actionDescriptor.GetParameters())
        //    {
        //        var p = new ParamMetadata()
        //            {
        //                Name = parmDescriptor.ParameterName,
        //                Comment = "Comment",
        //                Type = new ModelType()
        //                    {
        //                        Name = parmDescriptor.ParameterType.Name,
        //                        Properties = null
        //                    }
        //            };

        //        rtn.Add(p);
        //    }
        //    return rtn;
        //}

        //public static IEnumerable<dynamic> CreateMetadata(IEnumerable<ApiDescription> apiDescs, string rootControllerName)
        //{
        //    var rtnApis = new List<Models.Api>();
        //    foreach (var apiDescription in apiDescs)
        //    {
        //        var operations = new ApiOperation[]{};// CreateOperation(apiDescription);
        //        var currentControllerName = apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName;
        //        var alternatePath = PpsUtil.GetAlternatePath(rootControllerName, currentControllerName, apiDescription.RelativePath);

        //        Debug.WriteLine(currentControllerName + ":" + apiDescription.HttpMethod + ":" + alternatePath.ToLower() + ":" + apiDescription.RelativePath.ToLower());

        //        rtnApis.Add(new Models.Api()
        //        {
        //            path = alternatePath,
        //            description = apiDescription.Documentation,
        //            operations = operations,
        //        });
        //    }
        //    return rtnApis;
        //}


        public static List<string> GetRootControllers(List<string> allControllerNames)
        {
            var rootControllers = GetNamesWithOneUpper(allControllerNames).ToList();

            var likelySubControllers = allControllerNames.Where(name => name.ToCharArray().Count(c => Char.IsUpper(c)) > 1);

            var remainingControllers = new List<string>();
            foreach (var maybeSubController in likelySubControllers)
            {
                bool found = false;
                foreach (var rootName in rootControllers)
                {
                    if (maybeSubController.StartsWith(rootName))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    remainingControllers.Add(maybeSubController);
                }
            }

            rootControllers.AddRange(remainingControllers);
            rootControllers = rootControllers.OrderBy(s => s).ToList();

            Debug.WriteLine("Root Controllers: " + rootControllers.Count);
            Debug.WriteLine("================");
            rootControllers.ForEach(x => Debug.WriteLine(x));


            var notRootCount = allControllerNames.RemoveAll(name => rootControllers.Contains(name));
            allControllerNames = allControllerNames.OrderBy(s => s).ToList();
            Debug.WriteLine("");
            Debug.WriteLine("Sub Controllers: " + allControllerNames.Count);
            Debug.WriteLine("====================");
            allControllerNames.ForEach(x => Debug.WriteLine(x));
            return rootControllers;
        }

        private static IEnumerable<string> GetNamesWithOneUpper(List<string> controllerNames)
        {
            return controllerNames.Where(name => name.ToCharArray().Count(c => Char.IsUpper(c)) < 2);
        }


        public static List<string> GetUniqueNames(IEnumerable<ApiDescription> uniqueControllers)
        {
            return uniqueControllers
                .Select(c => c.ActionDescriptor.ControllerDescriptor.ControllerName)
                .Distinct()
                .ToList();
        }

        public static string GetAlternatePath(string rootControllerName, string currentControllerName, string relativePath)
        {
            var path = "/" + relativePath;

            if (currentControllerName != rootControllerName && relativePath.Contains(currentControllerName))
            {
                var shortName = currentControllerName.Substring(rootControllerName.Length);
                path = "/" + rootControllerName + "/{id}/" + shortName;


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
