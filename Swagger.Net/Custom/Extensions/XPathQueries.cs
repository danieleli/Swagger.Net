using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Http.Controllers;

namespace Swagger.Net.Custom.Extensions
{
    public static class XPathQueries
    {
        public const string METHOD = "/doc/members/member[@name='M:{0}']";
        public const string TYPE = "/doc/members/member[@name='T:{0}']";
        public const string ENUM = "/doc/members/member[@name='F:{0}']";
        public const string MEMBER = "param[@name='{0}']";
        public const string PROPERTY = "/doc/members/member[contains(@name,'P:{0}')]";
        public const string NO_DOCS_FOUND = "No docs found.";  //"No documentation found."


        // Type
        public static string XPathQuery(this Type type)
        {
            return string.Format(XPathQueries.TYPE, type.FullName);
        }


        // PropertyInfo
        public static string XPathQuery(this PropertyInfo propInfo)
        {
            var fullName = propInfo.ReflectedType.FullName + "." + propInfo.Name;
            return string.Format(XPathQueries.PROPERTY, fullName);
        }

        #region -- ActionDescriptor --

        public static string XPathQuery(this HttpActionDescriptor action)
        {
            var myAction = action as ReflectedHttpActionDescriptor;
            if (myAction != null)
            {
                var methodSignature = GetMethodSignature(myAction.MethodInfo);
                var selectExpression = string.Format(XPathQueries.METHOD, methodSignature);
                return selectExpression;
            }
            return null;
        }

        private static string GetMethodSignature(MethodInfo method)
        {
            if (method.DeclaringType == null) return "Method.DeclaringType not found.";

            var name = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
            var parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                string[] parameterTypeNames = parameters.Select(param => TypeUtils.GetNullableTypeName(param.ParameterType.FullName)).ToArray();
                name += string.Format("({0})", string.Join(",", parameterTypeNames));
            }

            return name;
        }

        #endregion -- ActionDescriptor

    }


}
