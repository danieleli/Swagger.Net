using System.Collections.Generic;

//  https://github.com/wordnik/swagger-core/wiki/API-Declaration

namespace Swagger.Net.Models
{

    //{
    //  apiVersion: "0.2",
    //  swaggerVersion: "1.1",
    //  basePath: "http://petstore.swagger.wordnik.com/api",
    //  resourcePath: "/pet.{format}"
    //  apis: [...]
    //  models: [...]  
    //}
    public class ResourceDescription
    {
        public ResourceDescription()
        {
            this.apis = new List<Api>();
            this.models = new Dictionary<string, object>();
        }

        public string apiVersion { get; set; }
        public string swaggerVersion { get; set; }
        public string basePath { get; set; }
        public string resourcePath { get; set; }
        public List<Api> apis { get; private set; }
        public Dictionary<string, object> models { get; private set; }
    }



    //apis:[
    //  {
    //    path          :"/pet.{format}/{petId}",
    //    description   :"Operations about pets",
    //    operations    :[...]
    //  }
    //]
    public class Api
    {
        public Api()
        {
            this.operations = new List<Operation>();
        }
        public string path { get; set; }
        public string description { get; set; }
        public List<Operation> operations { get; private set; }
    }





    //      Operation: {
    //        httpMethod        :"GET",
    //        nickname          :"getPetById",
    //        responseClass     :"Pet",                             https://github.com/wordnik/swagger-core/wiki/datatypes
    //        parameters        :[ ... ]                            https://github.com/wordnik/swagger-core/wiki/parameters
    //        summary           :"Find pet by its unique ID"
    //        notes             : "Only Pets which you have permission to see will be returned",
    //        errorResponses    :[ ... ]                            https://github.com/wordnik/swagger-core/wiki/errors
    //      }
    public class Operation
    {
        public Operation()
        {
            this.parameters = new List<dynamic>();
            this.errorResponses = new List<ErrorResponse>();
        }
        public string httpMethod { get; set; }
        public string nickname { get; set; }
        public string responseClass { get; set; }
        public string summary { get; set; }
        public string notes { get; set; }
        public List<dynamic> parameters { get; private set; }
        public List<ErrorResponse> errorResponses { get; private set; }
    }


    //errorResponses: {
    //    code: 400,
    //    reason: "Raised if a user supplies an invalid username format"
    // } 
    public class ErrorResponse
    {
        public int code { get; set; }
        public string reason { get; set; }
    }


//        {
//        paramType: "path",
//        name: "petId",
//        description: "ID of pet that needs to be fetched",
//        dataType: "String",
//        required: true,
//        allowableValues: {
//            max: 10,
//            min: 0,
//            valueType: "RANGE"
//        },
//        allowMultiple: false
//        }
    public class Parameter
    {
        public string paramType { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public bool required { get; set; }
        public bool allowMultiple { get; set; }
        public AllowableValues allowableValues { get; set; }
    }

    public class AllowableValues
    {
        public int max { get; set; }
        public int min { get; set; }
        public string valueType { get; set; }
    }

 



}