namespace YourCarePortal.Models
{
    public class Authorisation
    {
        public string? AUTHKEY { get; set; }

        public string? SETTING_StatementsEnabled { get; set; }

        public string? SETTING_BudgetEnabled { get; set; }

        public string? SETTING_SupportplanEnabled { get; set; }

        public string? SETTING_FormsEnabled { get; set; }

        public string? SETTING_NDIS_StatementsEnabled { get; set; }

        public string? SETTING_NDIS_Quotes_Enabled { get; set; }

        public string? LinkedClientIDS { get; set; }
        
        public bool AuthorisationFailed { get; set; } = false;
    }
}
