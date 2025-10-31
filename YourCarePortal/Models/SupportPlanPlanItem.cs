using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class SupportPlanPlanItem
    {
        [JsonProperty("planItemName")]
        public string planItemName { get; set; }

        [JsonProperty("planItemType")]
        public string planItemType { get; set; }

        [JsonProperty("planItemValue")]
        public string planItemValue { get; set; }
    }
}
