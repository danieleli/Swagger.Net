using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;

namespace Swagger.Net
{
    /// <summary>
    /// Accesses the XML doc blocks written in code to further document the API.
    /// All credit goes to: <see cref="http://blogs.msdn.com/b/yaohuang1/archive/2012/05/21/asp-net-web-api-generating-a-web-api-help-page-using-apiexplorer.aspx"/>
    /// </summary>
    public class XmlCommentDocumentationProvider : IDocumentationProvider
    {
        const string METHOD_EXPRESSION = "/doc/members/member[@name='M:{0}']";
        const string TYPE_EXPRESSION = "/doc/members/member[@name='T:{0}']";
        const string TYPE_MEMBERS_EXPRESSION = "/doc/members/member[contains(@name,'P:{0}')]";

        readonly XPathNavigator _documentNavigator;
        static readonly Regex NullableTypeNameRegex = new Regex(@"(.*\.Nullable)" + Regex.Escape("`1[[") + "([^,]*),.*");


        public XmlCommentDocumentationProvider(string documentPath)
        {
            var xpath = new XPathDocument(documentPath);
            _documentNavigator = xpath.CreateNavigator();
        }

        public virtual string GetDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            var reflectedParameterDescriptor = parameterDescriptor as ReflectedHttpParameterDescriptor;
            if (reflectedParameterDescriptor != null)
            {
                var memberNode = GetMemberNode(reflectedParameterDescriptor.ActionDescriptor);
                if (memberNode != null)
                {
                    var parameterName = reflectedParameterDescriptor.ParameterInfo.Name;
                    var parameterNode = memberNode.SelectSingleNode(string.Format("param[@name='{0}']", parameterName));
                    if (parameterNode != null)
                    {
                        return parameterNode.Value.Trim();
                    }
                }
            }

            return "No Documentation Found.";
        }

        public virtual bool GetRequired(HttpParameterDescriptor parameterDescriptor)
        {
            var reflectedParameterDescriptor = parameterDescriptor as ReflectedHttpParameterDescriptor;
            if (reflectedParameterDescriptor != null)
            {
                return !reflectedParameterDescriptor.ParameterInfo.IsOptional;
            }

            return true;
        }

        public virtual string GetDocumentation(HttpActionDescriptor actionDescriptor)
        {
            var memberNode = GetMemberNode(actionDescriptor);
            if (memberNode != null)
            {
                var summaryNode = memberNode.SelectSingleNode("summary");
                if (summaryNode != null)
                {
                    return summaryNode.Value.Trim();
                }
            }

            return "No Documentation Found.";
        }

        public virtual string GetNotes(HttpActionDescriptor actionDescriptor)
        {
            var memberNode = GetMemberNode(actionDescriptor);
            if (memberNode != null)
            {
                var summaryNode = memberNode.SelectSingleNode("remarks");
                if (summaryNode != null)
                {
                    return summaryNode.Value.Trim();
                }
            }

            return "No Documentation Found.";
        }

        public virtual string GetResponseClass(HttpActionDescriptor actionDescriptor)
        {
            var reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor != null)
            {
                if (reflectedActionDescriptor.MethodInfo.ReturnType.IsGenericType)
                {
                    var sb = new StringBuilder(reflectedActionDescriptor.MethodInfo.ReturnParameter.ParameterType.Name);
                    sb.Append("<");
                    Type[] types = reflectedActionDescriptor.MethodInfo.ReturnParameter.ParameterType.GetGenericArguments();
                    for (int i = 0; i < types.Length; i++)
                    {
                        sb.Append(types[i].Name);
                        if (i != (types.Length - 1)) sb.Append(", ");
                    }
                    sb.Append(">");
                    return sb.Replace("`1", "").ToString();
                }
                else
                    return reflectedActionDescriptor.MethodInfo.ReturnType.Name;
            }

            return "void";
        }

        public virtual string GetNickname(HttpActionDescriptor actionDescriptor)
        {
            var reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor != null)
            {
                return reflectedActionDescriptor.MethodInfo.Name;
            }

            return "NicknameNotFound";
        }

        private XPathNavigator GetMemberNode(HttpActionDescriptor actionDescriptor)
        {
            var reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor != null)
            {
                string selectExpression = string.Format(METHOD_EXPRESSION, GetMemberName(reflectedActionDescriptor.MethodInfo));
                var node = _documentNavigator.SelectSingleNode(selectExpression);
                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }

        private static string GetMemberName(MethodInfo method)
        {
            string name = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
            var parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                string[] parameterTypeNames = parameters.Select(param => ProcessTypeName(param.ParameterType.FullName)).ToArray();
                name += string.Format("({0})", string.Join(",", parameterTypeNames));
            }

            return name;
        }

        private static string ProcessTypeName(string typeName)
        {
            //handle nullable
            var result = NullableTypeNameRegex.Match(typeName);
            if (result.Success)
            {
                return string.Format("{0}{{{1}}}", result.Groups[1].Value, result.Groups[2].Value);
            }
            return typeName;
        }

        public string GetDocumentation(XPathNavigator node)
        {
            if (node == null) return null;

            var documentation = new JObject()
										{
											{ "summary", string.Empty },
											{ "example", string.Empty },
											{ "remarks", string.Empty },
											{ "returns", string.Empty },
											{ "ready", string.Empty },
											{ "datatype", string.Empty },
										};

            var summaryNode = node.SelectSingleNode("summary");
            var exampleNode = node.SelectSingleNode("example");
            var remarksNode = node.SelectSingleNode("remarks");
            var returnsNode = node.SelectSingleNode("returns");
            var readyNode = node.SelectSingleNode("ready");
            var dataTypeNode = node.SelectSingleNode("datatype");

            if (summaryNode != null)
            {
                documentation["summary"] = summaryNode.InnerXml.Trim();
            }

            if (exampleNode != null)
            {
                documentation["example"] = exampleNode.InnerXml.Trim();
            }

            if (remarksNode != null)
            {
                documentation["remarks"] = remarksNode.InnerXml.Trim();
            }

            if (returnsNode != null)
            {
                documentation["returns"] = returnsNode.InnerXml.Trim();
            }

            if (readyNode != null)
            {
                documentation["ready"] = readyNode.InnerXml.Trim();
            }

            if (dataTypeNode != null)
            {
                documentation["datatype"] = dataTypeNode.InnerXml.Trim();
            }

            return documentation.ToString();
        }

        public ApiModel GetApiModel(string modelName)
        {
            var apiModel = new ApiModel();
            apiModel.Name = modelName;

            var modelNode = this.GetTypeNode(modelName);

            if (modelNode != null)
            {
                apiModel.Documentation =  GetDocumentation(modelNode);

                var propertyNodes = GetTypeMemberNodes(modelName);
                foreach (XPathNavigator propertyNode in propertyNodes)
                    apiModel.Members.Add(new ApiModelMember()
                    {
                        Name = propertyNode.GetAttribute("name", "").Replace("P:" + modelName + ".", ""),
                        Documentation = GetDocumentation(propertyNode),
                    });
            }
            else
            {
                apiModel.Documentation = "No Documentation Found.";
            }

            return apiModel;
        }
        private XPathNavigator GetTypeNode(string typeName)
        {
            var selectExpression = string.Format(TYPE_EXPRESSION, typeName);
            var node = _documentNavigator.SelectSingleNode(selectExpression);

            return node;

        }

        private XPathNodeIterator GetTypeMemberNodes(string typeName)
        {
            var selectExpression = string.Format(TYPE_MEMBERS_EXPRESSION, typeName);
            var node = _documentNavigator.Select(selectExpression);

            return node;

        }
    }
}
