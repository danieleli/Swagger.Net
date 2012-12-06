using System.Collections.Generic;
//
// https://github.com/wordnik/swagger-core/wiki/Api-Declaration > APIs
//
namespace Swagger.Net.Models
{

    public class ApiDeclaration
    {
        public ApiDeclaration()
        {
            this.apis = new List<Api>();
            this.models = new Dictionary<string, object>();
        }

        public string apiVersion { get; set; }
        public string swaggerVersion { get; set; }
        public string basePath { get; set; }
        public string resourcePath { get; set; }
        public string description { get; set; }
        public List<Api> apis { get; set; }
        public Dictionary<string, object> models { get; set; }
    }

    public class Api
      {
        public string path { get; set; }
        public string description { get; set; }
        public IEnumerable<ApiOperation> operations { get; set; }
    }

}

