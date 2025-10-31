using Newtonsoft.Json;
using System.Collections.Generic;

namespace YourCarePortal.Models 
{ 
    public class RootNDISQuotes
    { 

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("qh_Version")]
        public string QhVersion { get; set; }

        [JsonProperty("qh_Deleted")]
        public string QhDeleted { get; set; }

        [JsonProperty("qh_Status")]
        public string QhStatus { get; set; }

        [JsonProperty("qh_CompanyNo")]
        public string QhCompanyNo { get; set; }

        [JsonProperty("qh_Date")]
        public string QhDate { get; set; }

        [JsonProperty("qh_UserNo")]
        public string QhUserNo { get; set; }

        [JsonProperty("qh_UserName")]
        public string QhUserName { get; set; }

        [JsonProperty("qh_ClientNo")]
        public string QhClientNo { get; set; }

        [JsonProperty("qh_ClientName")]
        public string QhClientName { get; set; }

        [JsonProperty("qh_ClientNDISNumber")]
        public string QhClientNDISNumber { get; set; }

        [JsonProperty("qh_ClientState")]
        public object QhClientState { get; set; }

        [JsonProperty("qh_Description")]
        public string QhDescription { get; set; }

        [JsonProperty("qh_RateIdx")]
        public string QhRateIdx { get; set; }

        [JsonProperty("qh_Start")]
        public string QhStart { get; set; }

        [JsonProperty("qh_End")]
        public string QhEnd { get; set; }

        [JsonProperty("qh_Amount")]
        public string QhAmount { get; set; }

        [JsonProperty("qh_Locked")]
        public string QhLocked { get; set; }

        [JsonProperty("qh_Signature")]
        public object QhSignature { get; set; }

        [JsonProperty("qh_ClientSigner")]
        public object Qh_ClientSigner { get; set; }

        [JsonProperty("qh_ClientSignDate")]
        public string Qh_ClientSignDate { get; set; }

        [JsonProperty("qh_AgentSignature")]
        public object Qh_AgentSignature { get; set; }

        [JsonProperty("qh_AgentSigner")]
        public object Qh_AgentSigner { get; set; }

        [JsonProperty("qh_AgentSignDate")]
        public DateTime Qh_AgentSignDate { get; set; }

        [JsonProperty("qh_Create")]
        public string QhCreate { get; set; }

        [JsonProperty("qh_Modify")]
        public string QhModify { get; set; }

        [JsonProperty("img_AgentSignature")]
        public string Img_AgentSignature { get; set; }

        [JsonProperty("img_ClientSignature")]
        public string Img_ClientSignature { get; set; }

        [JsonProperty("lines")]
        public List<RootNDISQuotesLine> Lines { get; set; }

        [JsonProperty("signable")]
        public string Signable { get; set; }

        [JsonProperty("summary")]
        public List<RootNDISQuotesSummary> Summary { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("terms")]
        public string Terms { get; set; }
    }
}
