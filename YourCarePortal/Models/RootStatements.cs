using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootStatements
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("showMenu")]
        public string ShowMenu { get; set; } // You can also consider using bool data type if the value is always true or false.

        [JsonProperty("pageTitle")]
        public string PageTitle { get; set; }

        [JsonProperty("currentBalance")]
        public string CurrentBalance { get; set; } // Use decimal if you plan on performing calculations on it

        [JsonProperty("clientFullname_ACTIVE_SESSION")]
        public string clientFullname_ACTIVE_SESSION { get; set; }

        [JsonProperty("clientID_ACTIVE_SESSION")]
        public string clientID_ACTIVE_SESSION { get; set; }

        [JsonProperty("dateLocked")]
        public string DateLocked { get; set; } // You can consider using DateTime data type if this will always be a date.

        [JsonProperty("statementsList")]
        public List<RootObjectStatements> StatementsList { get; set; }
    }
}
