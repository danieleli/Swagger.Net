using System.Collections.Generic;

//  https://github.com/wordnik/swagger-core/wiki/API-Declaration

namespace Swagger.Net.Models
{
    public class AllowableValues
    {
        public int max { get; set; }
        public int min { get; set; }
        public string valueType { get; set; }
    }
}