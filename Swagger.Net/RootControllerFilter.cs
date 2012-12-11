using System;
using System.Collections.Generic;
using System.Linq;

namespace Swagger.Net
{
    public class RootControllerFilter
    {
        public static IList<string> GetRootControllers(string[] distinctControllerNames)
        {
            var oneUpperCaseInName = GetNamesWithOneUpper(distinctControllerNames).ToArray();
            var moreThanOneUpperCaseInName = GetNamesWithManyUpper(distinctControllerNames);

            var rootControllers = GetRootControllers(moreThanOneUpperCaseInName, oneUpperCaseInName);

            return rootControllers;
        }

        private static List<string> GetRootControllers(IEnumerable<string> moreThanOneUpperCaseInName, string[] oneUpperCaseInName)
        {
            var remainingControllers = GetManyUpperCaseNoMatch_OneUpperCase(moreThanOneUpperCaseInName, oneUpperCaseInName);

            var rootControllers = new List<string>();

            rootControllers.AddRange(oneUpperCaseInName);
            rootControllers.AddRange(remainingControllers);

            rootControllers = rootControllers.OrderBy(s => s).ToList();
            return rootControllers;
        }

        private static IEnumerable<string> GetNamesWithOneUpper(IEnumerable<string> controllerNames)
        {
            return controllerNames.Where(name => name.ToCharArray().Count(c => Char.IsUpper(c)) < 2);
        }

        private static IEnumerable<string> GetNamesWithManyUpper(IEnumerable<string> controllerNames)
        {
            return controllerNames.Where(name => name.ToCharArray().Count(c => Char.IsUpper(c)) > 1);
        }

        private static IEnumerable<string> GetManyUpperCaseNoMatch_OneUpperCase(IEnumerable<string> namesWithManyUppers, string[] namesWithOneUpper)
        {
            var noMatches = new List<string>();
            foreach (var maybeMatch in namesWithManyUppers)
            {
                bool found = false;
                foreach (var rootName in namesWithOneUpper)
                {
                    if (maybeMatch.StartsWith(rootName))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    noMatches.Add(maybeMatch);
                }
            }
            return noMatches;
        }
    }
}