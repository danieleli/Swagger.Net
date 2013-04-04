using Newtonsoft.Json;

namespace Swagger.Net.Models
{
    public class ApiReference
    {
        /// <summary>
        /// Property equivalent to JSON's $Ref
        /// </summary>
        [JsonProperty(PropertyName = "$ref")]
        public string Reference { get; set; }
    }
}
