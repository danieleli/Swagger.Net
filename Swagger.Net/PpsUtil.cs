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
