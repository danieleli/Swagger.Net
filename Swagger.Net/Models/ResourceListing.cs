using System.Collections.Generic;

namespace Swagger.Net
{

    //{
    //  apiVersion: "0.2",
    //  swaggerVersion: "1.1",
    //  basePath: "http://petstore.swagger.wordnik.com/api",
    //  apis: [
    //    {
    //      path: "/pet.{format}",
    //      description: "Operations about pets"
    //    },
    //    {
    //      path: "/user.{format}",
    //      description: "Operations about user"
    //    }
    //  ]
    //}

    public class ResourceListing
    {
        public ResourceListing()
        {
            this.apis = new List<ResourceSummary>();
        }
        public string apiVersion { get; set; }
        public string swaggerVersion { get; set; }
        public string basePath { get; set; }
        public string resourcePath { get; set; }
        public IList<ResourceSummary> apis { get; private set; }
    }


    //apis:[
    //  {
    //    path          :"/pet.{format}",
    //    description   :"Operations about pets",
    //  }
    //]
    public class ResourceSummary
    {
        public string path { get; set; }
        public string description { get; set; }
    }

}