using System.Reflection;
using System.Web.Http.Controllers;
using System.Xml.XPath;

namespace Swagger.Net.Custom.Extensions
{
    public static class ParamExtensions
    {

        public static ParamMetadata GetDocs(this HttpParameterDescriptor param, XPathNavigator actionDocs)
        {
            var rtn = new ParamMetadata
                {
                    Name = param.ParameterName,
                    Comment = Utils.GetNodeValue(actionDocs, "param"),
                    Type = param.ParameterType.Name
                };
            
            return rtn;
        }


    }
}