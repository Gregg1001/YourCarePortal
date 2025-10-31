using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootSettingsNotification
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("notifications")]
        public string Notifications { get; set; }
    }
}
