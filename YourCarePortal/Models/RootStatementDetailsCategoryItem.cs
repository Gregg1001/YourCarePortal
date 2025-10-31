using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootStatementDetailsCategoryItem
    {
        [JsonProperty("statementItemName")]
        public string StatementItemName { get; set; }

        [JsonProperty("statementItemAmount")]
        public string StatementItemAmount { get; set; }
    }
}
