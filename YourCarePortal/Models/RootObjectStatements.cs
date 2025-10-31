using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootObjectStatements
    {
        [JsonProperty("month")]
        public string Month { get; set; }

        [JsonProperty("openingBalance")]
        public string OpeningBalance { get; set; } // Use decimal if you plan on performing calculations on it.

    }
}
