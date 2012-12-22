using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;
using Swagger.Net.Models;

namespace Swagger.Net
{
    /// <summary>
    /// Accesses the XML doc blocks written in code to further document the API.
    /// All credit goes to: <see cref="http://blogs.msdn.com/b/yaohuang1/archive/2012/05/21/asp-net-web-api-generating-a-web-api-help-page-using-apiexplorer.aspx"/>
    /// </summary>
    public class XmlCommentDocumentationProvider : IDocumentationProvider
    {
        const string METHOD_XPATH_QUERY = "/doc/members/member[@name='M:{0}']";
        const string TYPE_XPATH_QUERY = "/doc/members/member[@name='T:{0}']";
        const string ENUM_ITEM_XPATH_QUERY = "/doc/members/member[@name='F:{0}']";
        const string MEMBER_PARAM_XPATH_QUERY = "param[@name='{0}']";
        const string PROPERTY_XPATH_QUERY = "/doc/members/member[contains(@name,'P:{0}')]";
        const string NO_DOCS_FOUND = "";  //"No documentation found."


        #region --- fields & ctors ---


        readonly XPathNavigator _documentNavigator;
        
        public XmlCommentDocumentationProvider(string documentPath)
        {
            var xpath = new XPathDocument(documentPath);
            _documentNavigator = xpath.CreateNavigator();
        }

        #endregion --- fields & ctors ---

        public string GetDocumentation(XPathNavigator node)
        {
            if (node == null) return null;
            var jsonObj = new JObject();
            var elements = new List<string>() { "summary", "example", "remarks", "returns", "ready", "datatype" };

            elements.ForEach(e =>
            {
                var selectSingleNode = node.SelectSingleNode(e);
                if (selectSingleNode != null)
                    jsonObj[e] = selectSingleNode.InnerXml.Trim();
            });

            return jsonObj.ToString();
        }

        public string GetDocumentation(Type type)
        {
            try
            {
                var selector = string.Format(TYPE_XPATH_QUERY, type.FullName);
                var modelNode = _documentNavigator.SelectSingleNode(selector);

                return GetNodeText(modelNode, "summary");
            }
            catch (Exception)
            {

                return NO_DOCS_FOUND;
            }
            
        }

        public string GetDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            var parameterName = parameterDescriptor.ParameterName;
            var selector = string.Format(MEMBER_PARAM_XPATH_QUERY, parameterName);

            var rtn = GetActionDocumentation(parameterDescriptor.ActionDescriptor, selector);
            return rtn;
        }

        public string GetDocumentation(HttpActionDescriptor actionDescriptor)
        {
            var rtn = GetActionDocumentation(actionDescriptor, "summary");
            return rtn;
        }

        public string GetDocumentation(PropertyInfo propInfo)
        {
            var type = propInfo.ReflectedType;
            try
            {
                var selector = string.Format(PROPERTY_XPATH_QUERY, type.FullName + "." + propInfo.Name);
                var modelNode = _documentNavigator.SelectSingleNode(selector);

                return GetNodeText(modelNode, "summary");
            }
            catch (Exception)
            {

                return NO_DOCS_FOUND;
            }
        }

        public string GetRemarks(HttpActionDescriptor actionDescriptor)
        {
            return GetActionDocumentation(actionDescriptor, "remarks");
        }

        public string GetRemarks(Type type)
        {
            try
            {
                var selector = string.Format(TYPE_XPATH_QUERY, type.FullName);
                var modelNode = _documentNavigator.SelectSingleNode(selector);

                return GetNodeText(modelNode, "remarks");
            }
            catch (Exception)
            {
                return NO_DOCS_FOUND;
            }
        }

        public string GetResponseClass(HttpActionDescriptor actionDescriptor)
        {
            var reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor == null) return "void";

            var methodInfo = reflectedActionDescriptor.MethodInfo;

            var rtnClass = methodInfo.ReturnType.IsGenericType
                               ? GetGenericResponseClass(methodInfo)
                               : methodInfo.ReturnType.Name;


            return rtnClass;
        }

        private string GetActionDocumentation(HttpActionDescriptor actionDescriptor, string selector)
        {
            var methodNode = GetMethodNode(actionDescriptor);
            return GetNodeText(methodNode, selector);
        }

        private XPathNavigator GetMethodNode(HttpActionDescriptor actionDescriptor)
        {
            var action = actionDescriptor as ReflectedHttpActionDescriptor;
            if (action != null)
            {
                var methodSignature = GetMethodSignature(action.MethodInfo);
                var selectExpression = string.Format(METHOD_XPATH_QUERY, methodSignature);
                var node = _documentNavigator.SelectSingleNode(selectExpression);
                return node;
            }

            return null;
        }

        private static string GetMethodSignature(MethodInfo method)
        {
            var name = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
            var parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                string[] parameterTypeNames = parameters.Select(param => TypeUtils.GetNullableTypeName(param.ParameterType.FullName)).ToArray();
                name += string.Format("({0})", string.Join(",", parameterTypeNames));
            }

            return name;
        }

        private static string GetNodeText(XPathNavigator node, string selector)
        {
            if (node != null)
            {
                var summaryNode = node.SelectSingleNode(selector);
                if (summaryNode != null)
                {
                    return summaryNode.Value.Trim();
                }   
            }

            return NO_DOCS_FOUND;
        }

        private static string GetGenericResponseClass(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnParameter == null) return methodInfo.ReturnType.Name;
            
            var paramType = methodInfo.ReturnParameter.ParameterType;
            var argNames = string.Join(",", paramType.GetGenericArguments().Select(arg => arg.Name));
            
            var rtn = string.Format("{0}<{1}>", paramType.Name, string.Join(",", argNames));

            return rtn.Replace("`1", "");
        }


    }

    public static class TypeUtils
    {
        static readonly Regex NullableTypeNameRegex = new Regex(@"(.*\.Nullable)" + Regex.Escape("`1[[") + "([^,]*),.*");
        public static string GetNullableTypeName(string typeName)
        {
            //handle nullable
            var result = NullableTypeNameRegex.Match(typeName);
            if (result.Success)
            {
                return String.Format("{0}{{{1}}}", result.Groups[1].Value, result.Groups[2].Value);
            }
            return typeName;
        }
    }


}
