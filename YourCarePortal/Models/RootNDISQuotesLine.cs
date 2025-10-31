using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace YourCarePortal.Models
{
    public class RootNDISQuotesLine
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ql_CompanyNo")]
        public string CompanyNo { get; set; }

        [JsonProperty("ql_HeaderId")]
        public string HeaderId { get; set; }

        [JsonProperty("ql_Seqno")]
        public string Seqno { get; set; }

        [JsonProperty("ql_Start")]
        public string Start { get; set; }

        [JsonProperty("ql_End")]
        public string End { get; set; }

        [JsonProperty("ql_ItemCode")]
        public string ItemCode { get; set; }

        [JsonProperty("ql_SplitCode")]
        public string SplitCode { get; set; }

        [JsonProperty("ql_SelectedDays")]
        public string SelectedDays { get; set; }

        [JsonProperty("ql_Frequency")]
        public string Frequency { get; set; }

        [JsonProperty("ql_DaysNorm")]
        public string DaysNorm { get; set; }

        [JsonProperty("ql_QtyNorm")]
        public string QtyNorm { get; set; }

        [JsonProperty("ql_RateNorm")]
        public string RateNorm { get; set; }

        [JsonProperty("ql_DaysHoliday")]
        public string DaysHoliday { get; set; }

        [JsonProperty("ql_QtyHoliday")]
        public string QtyHoliday { get; set; }

        [JsonProperty("ql_RateHoliday")]
        public string RateHoliday { get; set; }

        [JsonProperty("ql_Amount")]
        public string Amount { get; set; }

        [JsonProperty("ql_FundingSource")]
        public string FundingSource { get; set; }

        [JsonProperty("ql_Notes")]
        public string Notes { get; set; }

        [JsonProperty("ql_Create")]
        public string Create { get; set; }

        [JsonProperty("ql_Modify")]
        public string Modify { get; set; }

        [JsonProperty("fam_Version")]
        public string FamVersion { get; set; }

        [JsonProperty("fam_BaseCode")]
        public string FamBaseCode { get; set; }

        [JsonProperty("fam_BaseName")]
        public string FamBaseName { get; set; }

        [JsonProperty("fam_GroupCode")]
        public string FamGroupCode { get; set; }

        [JsonProperty("fam_GroupName")]
        public string FamGroupName { get; set; }

        [JsonProperty("fam_CategoryCode")]
        public int FamCategoryCode { get; set; }

        [JsonProperty("fam_CategoryName")]
        public string FamCategoryName { get; set; }

        [JsonProperty("fam_Create")]
        public string FamCreate { get; set; }

        [JsonProperty("fam_Modify")]
        public string FamModify { get; set; }
    }
}
