using System.Collections.Generic;

namespace Swagger.Net
{

    /// <summary>
    /// Lists the resource endpoints available
    /// </summary>
    /// <example>
    /// {
    ///  apiVersion: "0.2",
    ///  swaggerVersion: "1.1",
    ///  basePath: "http://petstore.swagger.wordnik.com/api",
    ///  apis: [
    ///    {
    ///      path: "/pet.{format}",
    ///      description: "Operations about pets"
    ///    },
    ///    {
    ///      path: "/user.{format}",
    ///      description: "Operations about user"
    ///    }
    ///  ]
    /// }
    /// </example>
    public class ResourceListing
    {
        public ResourceListing()
        {
            this.apis = new List<ResourceSummary>();
        }

        /// <summary>
        /// Source: Executing assembly's assembly version (not file version)
        /// </summary>
        public string apiVersion { get; set; }

        /// <summary>
        /// Source: SwaggerConsts.SWAGGER_VERSION
        /// </summary>
        public string swaggerVersion { get; set; }


        /// <summary>
        /// Source: Request Uri Root + HttpRuntime.AppDomainAppVirtualPath
        /// </summary>
        public string basePath { get; set; }

        ///// <summary>
        ///// Source: Null
        ///// </summary>
        //public string resourcePath { get; set; }

        /// <summary>
        /// Source:  GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions
        /// </summary>
        public List<ResourceSummary> apis { get; private set; }
    }


   
    /// <summary>
    /// Source: GlobalConfiguration.Configuration.Services.GetApiExplorer().ApiDescriptions
    /// </summary>
    /// <example>
    ///     apis:[
    ///      {
    ///        path          :"/pet.{format}",
    ///        description   :"Operations about pets",
    ///      }
    ///    ] 
    /// </example>
    public class ResourceSummary
    {
        /// <summary>
        /// Source:  ApiDescription.RelativePath
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// Source: ApiDescription.Documentation 
        ///     passes thru to: XmlCommentDocumentationProvider.GetDocumentation(HttpActionDescriptor actionDescriptor)
        /// </summary>
        public string description { get; set; }
    }

}