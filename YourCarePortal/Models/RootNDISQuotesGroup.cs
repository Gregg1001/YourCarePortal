using Humanizer;
using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootNDISQuotesGroup
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }
    }
}






