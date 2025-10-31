using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class NDISStatementSupportPurpose
    {
        [JsonProperty("Support Purpose Name")]
        public string SupportPurposeName { get; set; }

        [JsonProperty("Start Date")]
        public string StartDate { get; set; }

        [JsonProperty("End Date")]
        public string EndDate { get; set; }

        [JsonProperty("Spending Cap")]
        public string SpendingCap { get; set; }

        [JsonProperty("Total Invoiced")]
        public string TotalInvoiced { get; set; }
    }
}
