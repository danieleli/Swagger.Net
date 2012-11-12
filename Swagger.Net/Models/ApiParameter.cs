namespace Swagger.Net.Models
{
    public class ApiParameter
    {
        public string paramType { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public bool required { get; set; }
        public bool allowMultiple { get; set; }
    }

    public class ApiEnumParameter : ApiParameter
    {
        public AllowableValues allowableValues { get; set; }
    }
}