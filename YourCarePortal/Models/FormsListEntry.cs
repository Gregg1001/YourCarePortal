using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class FormsListEntry
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("modified")]
        public string Modified { get; set; }
    }
}
