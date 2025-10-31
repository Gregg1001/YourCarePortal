
using Newtonsoft.Json;
using System.Collections.Generic;

namespace YourCarePortal.Models
{
    public class RootCustomForms
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("customFormTypeID")]
        public string CustomFormTypeID { get; set; }

        [JsonProperty("customFormTypeTitle")]
        public string CustomFormTypeTitle { get; set; }

        [JsonProperty("companyID")]
        public string CompanyID { get; set; }

        [JsonProperty("customFormID")]
        public string CustomFormID { get; set; }

        [JsonProperty("customFormVersion")]
        public string CustomFormVersion { get; set; }

        [JsonProperty("customFormPublished")]
        public string CustomFormPublished { get; set; }

        [JsonProperty("customFormEnabled")]
        public string CustomFormEnabled { get; set; }

        [JsonProperty("customFormEntryStatus")]
        public string CustomFormEntryStatus { get; set; }

        [JsonProperty("cntSignaturesExist")]
        public string CntSignaturesExist { get; set; }

        [JsonProperty("cntSignaturesWanted")]
        public string CntSignaturesWanted { get; set; }

        [JsonProperty("fields")]
        public List<RootCustomFormsFields> RootCustomFormsFields { get; set; }
    }
}
