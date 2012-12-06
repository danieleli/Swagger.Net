using System.Collections.Generic;

namespace Swagger.Net.Models
{
    public class ApiOperation
    {
        public ApiOperation()
        {
            this.parameters = new List<ApiParameter>();
            this.errorResponses = new List<ErrorResponse>();
        }
        public string httpMethod { get; set; }
        public string nickname { get; set; }
        public string responseClass { get; set; }
        public string summary { get; set; }
        public string notes { get; set; }
        public List<ApiParameter> parameters { get;  set; }
        public List<ErrorResponse> errorResponses { get; set; }
        
    }

}




//      ApiOperation: {
//        httpMethod        :"GET",
//        nickname          :"getPetById",
//        responseClass     :"Pet",                             https://github.com/wordnik/swagger-core/wiki/datatypes
//        parameters        :[ ... ]                            https://github.com/wordnik/swagger-core/wiki/parameters
//        summary           :"Find pet by its unique ID"
//        notes             : "Only Pets which you have permission to see will be returned",
//        errorResponses    :[ ... ]                            https://github.com/wordnik/swagger-core/wiki/errors
//      }
