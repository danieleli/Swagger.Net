using System.Collections.Generic;

namespace Swagger.Net.Models
{
    // see https://github.com/wordnik/swagger-core/wiki/datatypes
    public class Model
    {
        public Model()
        {
            this.Members = new List<Properties>();
        }

        public string Name { get; set; }
        public string type { get; set; }
        public string description { get; set; }

        public List<Properties> Members { get; private set; }
    }

    public class Properties
    {
        public string Name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
    }
}