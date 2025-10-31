using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootSupportPlanPlan
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("showMenu")]
        public string ShowMenu { get; set; }

        [JsonProperty("pageTitle")]
        public string PageTitle { get; set; }

        [JsonProperty("clientFullname_ACTIVE_SESSION")]
        public string ClientFullnameActiveSession { get; set; }

        [JsonProperty("clientID_ACTIVE_SESSION")]
        public string ClientIdActiveSession { get; set; }

        [JsonProperty("planContents")]
        public List<SupportPlanPlanItem> planContents { get; set; }
    }
}
