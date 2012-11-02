using System.Collections.Generic;


//  https://github.com/wordnik/swagger-core/wiki/API-Declaration
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

    // or

    //{
    //  apiVersion: "0.2",
    //  swaggerVersion: "1.1",
    //  basePath: "http://petstore.swagger.wordnik.com/api",
    //  resourcePath: "/pet.{format}"
    //  
    //  ...
    //
    //  apis: [...]
    //  models: [...]  
    //}
    public class ResourceListing
    {
        public ResourceListing()
        {
            this.models = new List<ApiModel>();
        }
        public string apiVersion { get; set; }
        public string swaggerVersion { get; set; }
        public string basePath { get; set; }
        public string resourcePath { get; set; }
        public List<ResourceApi> apis { get; set; }
        public List<ApiModel> models { get; set; }
    }

    // see https://github.com/wordnik/swagger-core/wiki/datatypes
    public class ApiModel
    {
        public ApiModel()
        {
            this.Members = new List<ApiModelMember>();
        }

        public string Name { get; set; }
        public string type { get; set; }
        public string description { get; set; }

        public List<ApiModelMember> Members { get; set; }
    }


    public class ApiModelMember
    {
        public string Name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
    }





    //apis:[
    //  {
    //    path:"/pet.{format}/{petId}",
    //    description:"Operations about pets",
    //    operations:[
    //      {
    //        httpMethod:"GET",
    //        nickname:"getPetById",
    //        responseClass:"Pet",
    //        parameters:[ ... ]
    //        summary:"Find pet by its unique ID"
    //        notes: "Only Pets which you have permission to see will be returned",
    //        errorResponses:[ ... ]
    //      }
    //    ]
    //  }
    //]
    public class ResourceApi
    {
        public string path { get; set; }
        public string description { get; set; }
        public List<ResourceApiOperation> operations { get; set; }
    }

    public class ResourceApiOperation
    {
        public string httpMethod { get; set; }
        public string nickname { get; set; }
        public string responseClass { get; set; }
        public string summary { get; set; }
        public string notes { get; set; }
        public List<ResourceApiOperationParameter> parameters { get; set; }
    }

//          {
//            paramType: "path",
//            name: "petId",
//            description: "ID of pet that needs to be fetched",
//            dataType: "String",
//            required: true,
//            allowableValues: {
//              max: 10,
//              min: 0,
//              valueType: "RANGE"
//            },
//            allowMultiple: false
//          }
    public class ResourceApiOperationParameter
    {
        public string paramType { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public bool required { get; set; }
        public bool allowMultiple { get; set; }
        public OperationParameterAllowableValues allowableValues { get; set; }
    }

    public class OperationParameterAllowableValues
    {
        public int max { get; set; }
        public int min { get; set; }
        public string valueType { get; set; }
    }
}