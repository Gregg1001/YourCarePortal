using Newtonsoft.Json;
using System.Collections.Generic;

namespace YourCarePortal.Models
{
    public class RootNDISStatement
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("showMenu")]
        public bool ShowMenu { get; set; }

        [JsonProperty("pageTitle")]
        public string PageTitle { get; set; }

        [JsonProperty("clientFullname_ACTIVE_SESSION")]
        public string ClientFullnameActiveSession { get; set; }

        [JsonProperty("clientID_ACTIVE_SESSION")]
        public string ClientIdActiveSession { get; set; }

        [JsonProperty("supportPurposes")]
        public List<NDISStatementSupportPurpose> SupportPurposes { get; set; }
    }
}
