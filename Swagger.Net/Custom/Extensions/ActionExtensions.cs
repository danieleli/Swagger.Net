using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Xml.XPath;

namespace Swagger.Net.Custom.Extensions
{
    public static class ActionExtensions
    {
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


        public static OperationMetadata GetDocs(this HttpActionDescriptor action)
        {
            return GetDocs(action, DocNavigator.Instance);
        }

        public static OperationMetadata GetDocs(this HttpActionDescriptor actionDescriptor, XPathNavigator docs)
        {
            var node = docs.SelectSingleNode(actionDescriptor.XPathQuery());

            var rtn = new OperationMetadata();

            return rtn;
        }


    }
}