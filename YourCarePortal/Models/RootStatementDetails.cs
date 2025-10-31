using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootStatementDetails
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("showMenu")]
        public string ShowMenu { get; set; }

        [JsonProperty("pageTitle")]
        public string PageTitle { get; set; }

        [JsonProperty("Current Balance")]
        public string CurrentBalance { get; set; }

        [JsonProperty("Date Locked")]
        public string DateLocked { get; set; }

        [JsonProperty("statementContents")]
        public List<RootStatementDetailsContent> RootStatementDetailsContent { get; set; }
    }
}
