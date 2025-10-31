using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace YourCarePortal.Models
{
    public class RootNDISQuotesSummary
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fromCat")]
        public int FromCat { get; set; }

        [JsonProperty("toCat")]
        public int ToCat { get; set; }

        [JsonProperty("hideIfZero")]
        public bool HideIfZero { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonProperty("cats")]
        public List<RootNDISQuotesCategory> Cats { get; set; }
    }
}
