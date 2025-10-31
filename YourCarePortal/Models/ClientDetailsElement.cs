using Newtonsoft.Json;
using System.Xml.Linq;

namespace YourCarePortal.Models
{
    public class ClientDetailsElement
    {
        public string Type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Class { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Style { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientDetailsElement> Elements { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientDetailsRow> Rows { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Src { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Href { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Span { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Size { get; set; }

    }
}
