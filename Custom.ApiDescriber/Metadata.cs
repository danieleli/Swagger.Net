using System;
using System.Collections.Generic;

namespace Custom.ApiDescriber
{
    public abstract class Metadata
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
    }


    //public class AllowableValues
    //{
    //    public AllowableValues(Type myEnum)
    //    {
    //        valueType = "LIST";
    //        values = Enum.GetNames(myEnum);
    //    }

    //    public string[] values { get; set; }
    //    public int max { get; set; }
    //    public int min { get; set; }
    //    public string valueType { get; set; }
    //}
    
    // todo: implement EnumMetadata or allowableValues
    public class EnumMetadata : Metadata
    {
        public EnumMetadata(Type myEnum)
        {
            valueType = "LIST";
            values = Enum.GetNames(myEnum);
        }

        protected string[] values { get; set; }

        protected string valueType { get; set; }

    }

    public class TypeMetadata : Metadata
    {
        public IEnumerable<PropertyMetadata> Properties { get; set; }
    }

    public class PropertyMetadata : Metadata
    {
        public string DataType { get; set; }
        //public string Value { get; set; }
    }


    public class ParamMetadata
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public string Type { get; set; }
        //  public TypeMetadata Type { get; set; }
    }

    public class ActionMetadata : Metadata
    {
        public string ReturnsComment { get; set; }
        public TypeMetadata ReturnType { get; set; }
        public string RelativePath { get; set; }
        public string AlternatePath { get; set; }
        public string HttpMethod { get; set; }
        public IEnumerable<ParamMetadata> Params { get; set; }
        public IEnumerable<ErrorMetadata> ErrorResponses { get; set; }  // Todo: Populate Error Responses
    }




    public class ControllerMetadata : Metadata
    {
        public string ParentController { get; set; }
        public IEnumerable<ActionMetadata> Operations { get; set; }
        public IEnumerable<ControllerMetadata> Children { get; set; }
        public TypeMetadata ModelType { get; set; }
    }


    public class ModelMetadata : TypeMetadata
    {
        public ModelMetadata()
        {
            this.Samples = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Samples { get; set; }

    }

    public class ErrorMetadata
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

}