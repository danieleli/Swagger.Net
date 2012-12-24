using System.Reflection;
using System.Web.Http.Controllers;
using System.Xml.XPath;

namespace Swagger.Net.Custom.Extensions
{
    public static class ParamExtensions
    {

        public static ParamMetadata GetDocs(this HttpParameterDescriptor param, XPathNavigator actionDocs)
        {
            var pName = param.ParameterName;
            var comment = Utils.GetNodeValue(actionDocs, "param");
            var t = param.ParameterType.Name;
            var rtn = new ParamMetadata
                {
                    Name = pName,
                    Comment = comment,
                    Type = t
                };
            
            return rtn;
        }


    }
}