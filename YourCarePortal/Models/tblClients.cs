using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{
    public class tblClients
    {
        [Key]
        public int ClientID { get; set; }
        public string ClientRefer { get; set; }
        public string ClientRefNumber { get; set; }
        public int? ClientReferrerNum { get; set; }
        public int? ClientPackageNum { get; set; }
        public string ClientTitle { get; set; }
        public string ClientFullName { get; set; }
        public string ClientFirstname { get; set; }
        public string ClientSurname { get; set; }
        public string ClientDescription { get; set; }
        public int? ClientCompanyID { get; set; }
        public DateTime? ClientDOB { get; set; }
        public string ClientPropertyName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientSuburb { get; set; }
        public string ClientPostcode { get; set; }
        public string ClientState { get; set; }
        public string ClientCoordinates { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public string ClientMobile { get; set; }
        public DateTime? ClientDateModified { get; set; }
        public int? ClientCareLevel1 { get; set; }
        public int? ClientCareLevel2 { get; set; }
        public int? ClientCareLevel3 { get; set; }
        public int? ClientCareLevel4 { get; set; }
        public int? ClientCareLevel5 { get; set; }
        public string ClientExclusions { get; set; }
        public string ClientStatus1 { get; set; }
        public bool? ClientSystemAccess { get; set; }
        public string ClientSystemPassword { get; set; }
        public string ClientAccountingSystemRef { get; set; }
        public bool? ClientDisabled { get; set; }
        public string ClientNotesForStaff { get; set; }
        public string ClientPreferences { get; set; }
        public bool? ClientNonChargeable { get; set; }
        public int? ClientChargeableKMDefault { get; set; }
        public int? ClientKMsFromOffice { get; set; }
        public float? ClientBalance { get; set; }
        public string ClientBudgetStructure1 { get; set; }
        public bool? ClientStatementsEnabled { get; set; }
        public string ClientMedicareIdentifier { get; set; }
        public string ClientLanguagesSpoken { get; set; }
        public string ClientPackageSchedule { get; set; }
        public string ClientCarePlanStructure { get; set; }
        public DateTime? ClientStatementDateLocked { get; set; }
        public string ClientPrefname { get; set; }
        public string ClientDVAFileNumber { get; set; }
        public string ClientDVAPlanID { get; set; }
        public int? ClientCaseManager { get; set; }
        public string ClientReportingSettings1 { get; set; }
        public float? ClientBalance_Contingency { get; set; }
        public int? ClientBillingGroup { get; set; }
        public int? ClientSchedulingUser { get; set; }
        public string ClientSupportPlan1 { get; set; }
        public string ClientBankAccountDetails { get; set; }
        public string ClientBillingAddress { get; set; }
        public string ClientSettings_ABF { get; set; }
        public int? ClientChargeableMINSDefault { get; set; }
        public int? ClientRegion { get; set; }
        public string ClientCustomSettingsForm1 { get; set; }
        public string ClientCustomSettingsFormEmergency { get; set; }
        public string ClientAccountingSystemRef_Additional { get; set; }
        public int? ClientCountryofBirth { get; set; }
        public int? ClientLanguageMain { get; set; }
        public string ClientNDISNumber { get; set; }
        public int? ClientGender { get; set; }
        public int? ClientExcludeFromDEX { get; set; }
        public int? ClientStatementDelivery { get; set; }
        public string ClientPhotoPath1 { get; set; }
        public int? ClientCustomOption1 { get; set; }
        public int? ClientCustomOption2 { get; set; }
        public int? ClientCustomOption3 { get; set; }
        public int? ClientNDISPriceZone { get; set; }
        public DateTime? ClientDateAdded { get; set; }
        public string ClientPortalAccessSettings { get; set; }
        public int? ClientNAPSgroup { get; set; }
        public int? ClientStatus2 { get; set; }
        public Guid? ClientUUID { get; set; }
        public int? ClientPlanningRegion { get; set; }
        public int? ClientStatementLanguagePreference { get; set; }
        public int? ClientCustomOption4 { get; set; }
        public int? ClientCustomOption5 { get; set; }
        public int? ClientCustomOption6 { get; set; }
        public int? ClientCustomOption7 { get; set; }
        public int? ClientCustomOption8 { get; set; }
        public int? ClientCustomOption9 { get; set; }
        public int? ClientCustomOption10 { get; set; }
    }
}
