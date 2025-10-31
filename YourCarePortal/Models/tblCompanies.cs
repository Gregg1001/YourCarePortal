using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{
    public partial class tblCompanies
    {
        [Key]
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAbn { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanySuburb { get; set; }
        public string? CompanyPostcode { get; set; }
        public string? CompanyState { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyWebsite { get; set; }
        public DateTime? CompanyDateModified { get; set; }
        public string? CompanyInvoiceTerms { get; set; }
        public string? CompanyShiftDefinitions { get; set; }
        public double? CompanyRateBase { get; set; }
        public string? CompanyRateDefinitions { get; set; }
        public string? CompanyLogo { get; set; }
        public DateTime? CompanyPublishedDate { get; set; }
        public bool? CompanyCareOrganisation { get; set; }
        public bool? CompanyReferrer { get; set; }
        public bool? CompanyDisabled { get; set; }
        public string? CompanyCoords { get; set; }
        public string? CompanyRosterType { get; set; }
        public string? CompanyDefaultMobile { get; set; }
        public string? CompanyMobileTitle { get; set; }
        public int? CompanyAllowanceKm1 { get; set; }
        public int? CompanyAllowanceKm2 { get; set; }
        public bool? CompanyAccountsRestricted { get; set; }
        public string? CompanyPayrollConfig { get; set; }
        public int? CompanyLastInvoiceNumber { get; set; }
        public int? CompanyShiftBreakMins { get; set; }
        public string? CompanyCarePlanFormat { get; set; }
        public string? CompanyBankAccountNumber1 { get; set; }
        public string? CompanyBankAccountName1 { get; set; }
        public string? CompanyBankAccountBsb1 { get; set; }
        public string? CompanyPackageServiceRestrictions { get; set; }
        public int? CompanyHaccproviderNumber { get; set; }
        public string? CompanyDvanursingSenderCode { get; set; }
        public string? CompanyDvanursingServicingDoctor { get; set; }
        public string? CompanyAccountsEmail { get; set; }
        public string? CompanyNdisproviderNumber { get; set; }
        public int? CompanyPlannerVersion { get; set; }
        public int? CompanyGpswarningsEnforced { get; set; }
        public int? CompanyGeoFencing { get; set; }
        public string? CompanyAccountsConfig { get; set; }
        public string? CompanyGeneralConfig { get; set; }
        public string? CompanyAccountsInstructions { get; set; }
        public int? CompanyCrmid { get; set; }
        public int? CompanyPayrollFormat { get; set; }
        public int? CompanyInvoiceFormat { get; set; }
        public string? CompanyBillingConfig { get; set; }
        public DateTime? CompanyAvailabilityLockDate { get; set; }
    }
}
