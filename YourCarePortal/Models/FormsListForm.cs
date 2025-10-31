using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class FormsListForm
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("entrys")]
        public List<FormsListEntry> Entrys { get; set; }
    }
}
