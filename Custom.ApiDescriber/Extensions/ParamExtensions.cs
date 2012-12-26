using System.Web.Http.Controllers;
using System.Xml.XPath;

namespace Custom.ApiDescriber.Extensions
{
    public static class ParamExtensions
    {

        public static ParamMetadata GetDocs(this HttpParameterDescriptor param, XPathNavigator actionDocs)
        {
            var rtn = new ParamMetadata
                {
                    Name = param.ParameterName,
                    Comment = Utils.GetNodeValue(actionDocs, "param"),
                    Type = Utils.GetCleanTypeName(param.ParameterType)
                };
            
            return rtn;
        }


    }
}