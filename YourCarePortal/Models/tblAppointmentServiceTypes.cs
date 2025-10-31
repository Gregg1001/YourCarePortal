using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{
    public class tblAppointmentServiceTypes
    {
        [Key]
        public int AppointmentServiceTypeID { get; set; }
        public string AppointmentServiceTypeName { get; set; }
        public string AppointmentServiceTypeCode { get; set; }
        public string AppointmentServiceTypePayrollCode { get; set; }
        public int AppointmentServiceTypePackage { get; set; }
        public string AppointmentServiceTypeBilling { get; set; }
        public short AppointmentServiceTypeCompany { get; set; }
        public string AppointmentServiceTypeNotes { get; set; }
        public bool AppointmentServiceTypeDeleted { get; set; }
        public int AppointmentServiceTypePriority { get; set; }
        public float AppointmentServiceTypeDefaultRate { get; set; }
        public string AppointmentServiceTypePayType { get; set; }
        public string AppointmentServiceTypeDefaultQualifications { get; set; }
        public bool AppointmentServiceTypeAllowTransport { get; set; }
        public string AppointmentServiceTypeDVACode { get; set; }
        public string AppointmentServiceTypeHACCCode { get; set; }
        public int AppointmentServiceTypeDefaultRecurrence { get; set; }
        public int AppointmentServiceTypeTaxRate { get; set; }
        public string AppointmentServiceTypeDexcode { get; set; }
        public string AppointmentServiceTypeDVANursingConfig { get; set; }
        public string AppointmentServiceTypeTag { get; set; }
        public int AppointmentServiceTypeNDISGroup { get; set; }
        public int AppointmentServiceTypeNDISSupportItem { get; set; }
        public string AppointmentServiceTypePayrollClassification { get; set; }
        public int AppointmentServiceTypeNDIS_ActivityBasedTransport { get; set; }
        public bool AppointmentServiceTypeConflictsIgnore { get; set; }
        public int AppointmentServiceTypeNDIS_ProviderTravel { get; set; }
        public string AppointmentServiceTypeRateOverride { get; set; }
        public int AppointmentServiceTypeTravelRestriction { get; set; }
        public string AppointmentServiceTypeName_External1 { get; set; }
        public int AppointmentServiceTypeName_External1_Setting { get; set; }
    }

}
