using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Routing;
using Swagger.Net.Models;

namespace Swagger.Net.Factories
{
    /// <summary>
    /// | .net              | swagger           |
    /// -----------------------------------------
    /// | EndpointMetadata  | Resource Listing  |
    /// </summary>
    public class ResourceListingFactory
    {

        #region --- fields & ctors ---

        private readonly string _appVirtualPath;
        private readonly IEnumerable<ApiDescription> _apiDescriptions;

        public ResourceListingFactory()
        {
            _appVirtualPath = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/');
            _apiDescriptions = GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions;
        }

        public ResourceListingFactory(string appVirtualPath, IEnumerable<ApiDescription> apiDescs)
        {
            _appVirtualPath = appVirtualPath.TrimEnd('/');
            _apiDescriptions = apiDescs;
        }

        #endregion --- fields & ctors ---

        public ResourceListing CreateResourceListing(Uri uri)
        {
            var apiVersion = Assembly.GetCallingAssembly().GetName().Version.ToString();
            var basePath = uri.GetLeftPart(UriPartial.Authority) + _appVirtualPath;

            var rtnListing = new ResourceListing()
            {
                apiVersion = apiVersion,
                swaggerVersion = G.SWAGGER_VERSION,
                basePath = basePath + @"/api/docs",
                apis = CreateApis()
            };

            return rtnListing;
        }

        private Api[] CreateApis()
        {

            var uniqueControllers = _apiDescriptions
                .Select(api => api)
                .Distinct().ToList();

            var controllerNames = GetUniqueNames(uniqueControllers);
            var rootControllers = GetRootControllers(controllerNames);


            return (from api in uniqueControllers
                    where rootControllers.Contains(api.ActionDescriptor.ControllerDescriptor.ControllerName)
                    group api by api.ActionDescriptor.ControllerDescriptor.ControllerName
                    into g
                    select new Api
                               {
                                   description = g.First().Documentation,
                                   path = @"/" + g.Key
                               }
                   ).OrderBy(a=>a.path).ToArray();
        }

        private static List<string> GetRootControllers(List<string> controllerNames)
        {
            var rootControllers = GetNamesWithOneUpper(controllerNames).ToList();

            var controllerNamesWithManyUppers = controllerNames.Where(name => name.ToCharArray().Count(c => Char.IsUpper(c)) > 1);

            var extra = new List<string>();
            foreach (var controllerNameWithManyUpper in controllerNamesWithManyUppers)
            {
                bool found = false;
                foreach (var rootName in rootControllers)
                {
                    if (controllerNameWithManyUpper.StartsWith(rootName))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    extra.Add(controllerNameWithManyUpper);
                }
            }

            rootControllers.AddRange(extra);

            rootControllers = rootControllers.OrderBy(s => s).ToList();

            Debug.WriteLine("Root Controllers: " + rootControllers.Count);
            Debug.WriteLine("================");
            rootControllers.ForEach(x => Debug.WriteLine(x));


            var notRoot = controllerNames.RemoveAll(name => rootControllers.Contains(name));
            controllerNames = controllerNames.OrderBy(s => s).ToList();
            Debug.WriteLine("");
            Debug.WriteLine("Sub Controllers: " + controllerNames.Count);
            Debug.WriteLine("====================");
            controllerNames.ForEach(x => Debug.WriteLine(x));
            return rootControllers;
        }

        private static IEnumerable<string> GetNamesWithOneUpper(List<string> controllerNames)
        {
            return controllerNames.Where(name => name.ToCharArray().Count(c => Char.IsUpper(c))<2);
        }


        public static string GetRootName(string ctlrName)
        {
            int secondUpperAfterFirstLower = 0;
            var chars = ctlrName.ToCharArray();
            bool foundFirstLower = false;
            for (var i = 1; i < chars.Length - 1; i++)
            {
                if (foundFirstLower && Char.IsUpper(chars[i]))
                {
                    secondUpperAfterFirstLower = i;
                    break;
                }
                if (Char.IsLower(chars[i]))
                {
                    foundFirstLower = true;
                }

            }
            if (secondUpperAfterFirstLower == 0)
            {
                return ctlrName;
            }
            var rootName = ctlrName.Substring(0, secondUpperAfterFirstLower);
            return rootName;
        }

        private static List<string> GetUniqueNames(IEnumerable<ApiDescription> uniqueControllers)
        {
            return uniqueControllers
                .Select(c => c.ActionDescriptor.ControllerDescriptor.ControllerName)
                .Distinct()
                .ToList();
        }
    }
}


//name: "API Default",
//    routeTemplate: "{controller}/{id}",
//    defaults:
//new { id = RouteParameter.Optional }

//                name: "Resource extension",
//                routeTemplate: "{controller}/{id}.{ext}"


//                name: "Controller extension",
//                routeTemplate: "{controller}.{ext}"

//                name: "Sub Resource Route",
//                routeTemplate: "{controller}/{id}/{subcontroller}/{subid}",
//                defaults: new { subid = RouteParameter.Optional }


//                name: "Sub Resource extension",
//                routeTemplate: "{controller}/{id}/{subcontroller}/{subid}.{ext}"


//                name: "Sub Controller extension",
//                routeTemplate: "{controller}/{id}/{subcontroller}.{ext}"
