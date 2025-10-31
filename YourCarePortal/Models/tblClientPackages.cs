using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{

    public class tblClientPackages
    {
        [Key]
        public int ClientPackageId { get; set; }
        public string ClientPackageName { get; set; }
        public short? ClientPackageCompany { get; set; }
        public string ClientPackageNotes { get; set; }
        public bool? ClientPackageDeleted { get; set; }
        public string ClientPackageBillingCode { get; set; }
        public bool? ClientPackageChargedToReferrer { get; set; }
        public string ClientPackagePayrollCode { get; set; }
        public bool? ClientPackageHcpEnabled { get; set; }
        public int? ClientPackageFundingType { get; set; }
        public int? ClientPackageBillingType { get; set; }
        public string ClientPackageDexCaseId { get; set; }
        public string ClientPackageTag { get; set; }
        public string ClientPackageFundingBody { get; set; }
        public string ClientPackageAccountsConfig { get; set; }
        public int? ClientPackageSequence { get; set; }
    }

}
