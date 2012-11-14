using System.Collections.Generic;

namespace Swagger.Net.Models
{
    // see https://github.com/wordnik/swagger-core/wiki/datatypes
    public class ApiModel
    {
        public ApiModel()
        {
            this.properties = new Dictionary<string, object>();
        }

        public string Name { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string description { get; set; }

        public Dictionary<string, object> properties { get; set; }
    }

    public class ApiModelProperties
    {
        public string id { get; set; }
        public string type { get; set; }
        public string description { get; set; }
    }
}