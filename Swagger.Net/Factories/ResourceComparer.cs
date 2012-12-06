using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace Swagger.Net.Factories
{
    public class ResourceComparer : IEqualityComparer<HttpControllerDescriptor>
    {
        public bool Equals(HttpControllerDescriptor c1, HttpControllerDescriptor c2)
        {
            var c1Root = Util.GetRootName(c1.ControllerName);
            var c2Root = Util.GetRootName(c2.ControllerName);


            if (c1Root == c2Root)
            {
                return true;
            }
            return false;

        }



        public int GetHashCode(HttpControllerDescriptor descriptor)
        {
            var name = Util.GetRootName(descriptor.ControllerName);
            return name.GetHashCode();
        }


    }

    public class Util
    {
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
    }

    public class ApiComparer : IEqualityComparer<ApiDescription>
    {
        public bool Equals(ApiDescription d1, ApiDescription d2)
        {
            var c1Root = Util.GetRootName(d1.ActionDescriptor.ControllerDescriptor.ControllerName);
            var c2Root = Util.GetRootName(d2.ActionDescriptor.ControllerDescriptor.ControllerName);


            if (c1Root == c2Root)
            {
                return true;
            }
            return false;

        }




        public int GetHashCode(ApiDescription descriptor)
        {
            var name = Util.GetRootName(descriptor.ActionDescriptor.ControllerDescriptor.ControllerName);
            return name.GetHashCode();
        }


    }
}