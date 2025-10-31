using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace YourCarePortal.Models
{
    public class RootNDISQuotesCategory
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("groups")]
        public List<RootNDISQuotesGroup> Groups { get; set; }
    }
}
