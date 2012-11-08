using System.Collections.Generic;

namespace Swagger.Net.Models
{
    // see https://github.com/wordnik/swagger-core/wiki/datatypes
    public class Model
    {
        public Model()
        {
            this.properties = new List<Properties>();
        }

        public string Name { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string description { get; set; }

        public List<Properties> properties { get; private set; }
    }

    public class Properties
    {
        public string id { get; set; }
        public string type { get; set; }
        public string description { get; set; }
    }
}