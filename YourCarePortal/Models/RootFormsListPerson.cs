using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootFormsListPerson
    {
        [JsonProperty("cid")]
        public string Cid { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("companyid")]
        public string CompanyId { get; set; }

        [JsonProperty("forms")]
        public List<FormsListForm> Forms { get; set; }
    }
}
