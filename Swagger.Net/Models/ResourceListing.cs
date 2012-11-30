using System.Collections.Generic;
using Swagger.Net.Models;

// https://github.com/wordnik/swagger-core/wiki/Resource-Listing
namespace Swagger.Net
{
    public class ResourceListing
    {
        public string apiVersion { get; set; }
        public string swaggerVersion { get; set; }
        public string basePath { get; set; }
        public Api[] apis { get; set; }
    }

    public class Api
    {
        public string path { get; set; }
        public string description { get; set; }
    }
}


//   example:
//   {
//    apiVersion: "0.2",
//    swaggerVersion: "1.1",
//    basePath: "http://petstore.swagger.wordnik.com/api",
//    apis: [
//      {
//        path: "/pet.{format}",
//        description: "Operations about pets"
//      },
//      {
//        path: "/user.{format}",
//        description: "Operations about user"
//      }
//    ]
//   }
