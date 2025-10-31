using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootCustomFormsFields
    {
        
        [JsonProperty("customFormFieldOrder")]
        public string CustomFormFieldOrder { get; set; }

        [JsonProperty("customFormFieldID")]
        public string CustomFormFieldID { get; set; }

        [JsonProperty("customFormFieldTypeID")]
        public string CustomFormFieldTypeID { get; set; }

        [JsonProperty("customFormFieldName")]
        public string CustomFormFieldName { get; set; }

        [JsonProperty("customFormDataValue")]
        public string CustomFormDataValue { get; set; }

        [JsonProperty("customFormSignatureName")]
        public string CustomFormSignatureName { get; set; }

    }
}
