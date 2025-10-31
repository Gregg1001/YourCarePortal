using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootStatementDetailsContent
    {
        [JsonProperty("statementCategoryHeading")]
        public string StatementCategoryHeading { get; set; }

        [JsonProperty("statementCategoryContent")]
        public List<RootStatementDetailsCategoryItem> StatementCategoryContent { get; set; }

        [JsonProperty("statementCategoryTotal")]
        public string StatementCategoryTotal { get; set; }
    }
}
