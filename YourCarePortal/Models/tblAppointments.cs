namespace YourCarePortal.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class tblAppointments
    {
        [Key]
        public int AppointmentID { get; set; }
        public int? AppointmentCompanyID { get; set; }
        public int? AppointmentUserID { get; set; }
        public int? AppointmentClientID { get; set; }
        public string AppointmentQualificationLevel { get; set; }
        public string AppointmentReferenceNumber { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public DateTime? AppointmentStartTime { get; set; }
        public DateTime? AppointmentEndTime { get; set; }
        public int? AppointmentStartPriority { get; set; }
        public DateTime? AppointmentStartTime_Marked { get; set; }
        public DateTime? AppointmentEndTime_Marked { get; set; }
        public DateTime? AppointmentStartTime_Actual { get; set; }
        public DateTime? AppointmentEndTime_Actual { get; set; }
        public int? AppointmentDuration { get; set; }
        public string AppointmentNotes { get; set; }
        public string AppointmentNotes_Carer { get; set; }
        public string AppointmentNotes_Admin { get; set; }
        public float? AppointmentEstCarerCost { get; set; }
        public float? AppointmentCost { get; set; }
        public string AppointmentShiftCode { get; set; }
        public int? AppointmentStatus1 { get; set; }
        public int? AppointmentStatus2 { get; set; }
        public int? AppointmentStatus3 { get; set; }
        public string AppointmentStatusTags { get; set; }
        public string AppointmentTasks { get; set; }
        public string AppointmentTaskBreakdown { get; set; }
        public string AppointmentLastconfirmedStatus { get; set; }
        public string AppointmentMedications { get; set; }
        public string AppointmentMedicationDetails { get; set; }
        public string AppointmentEquipment { get; set; }
        public string AppointmentEquipmentDetails { get; set; }
        public DateTime? AppointmentLastUpdated { get; set; }
        public string AppointmentPhotos { get; set; }
        public bool? AppointmentRepeat { get; set; }
        public int? AppointmentParentRepeatID { get; set; }
        public DateTime? AppointmentRepeatStart { get; set; }
        public DateTime? AppointmentRepeatEnd { get; set; }
        public string AppointmentRepeatFrequency { get; set; }
        public float? AppointmentKM1 { get; set; }
        public float? AppointmentKM2 { get; set; }
        public string AppointmentFlexibility { get; set; }
        public float? AppointmentStatus4 { get; set; }
        public bool? AppointmentEndsNextDay { get; set; }
        public int? AppointmentPayrollCategory { get; set; }
        public int? AppointmentServiceType { get; set; }
        public string AppointmentExtraCharge1 { get; set; }
        public string AppointmentExtraCharge1_type { get; set; }
        public bool? AppointmentChargeableCancellation { get; set; }
        public string AppointmentChargeableCancellationDetails { get; set; }
        public int? AppointmentPackageOverride { get; set; }
        public bool? AppointmentTimeVarianceAccepted { get; set; }
        public string AppointmentRepeatWeekday { get; set; }
        public int? AppointmentRepeatWeekCount { get; set; }
        public int? AppointmentTravel1 { get; set; }
        public string AppointmentSignaturePath { get; set; }
        public string AppointmentVehicleType { get; set; }
        public bool? AppointmentExcludeFromPayroll { get; set; }
        public string AppointmentMultiUserIDs { get; set; }
        public bool? AppointmentMultiUser { get; set; }
        public string AppointmentRejectedUserIDS { get; set; }
        public int? AppointmentTransport_Pickup { get; set; }
        public int? AppointmentTransport_Dropoff { get; set; }
        public int? AppointmentBranch { get; set; }
        public string AppointmentMultiClientIDS { get; set; }
        public bool? AppointmentMultiClient { get; set; }
        public byte? AppointmentTravelVariance { get; set; }
        public string AppointmentTravelVarianceDetails { get; set; }
        public int? AppointmentLocationOverride { get; set; }
        public string AppointmentCancellationReason { get; set; }
        public string AppointmentCancellationReason_Note { get; set; }
        public bool? AppointmentEquipmentChargesApproved { get; set; }
        public string AppointmentCustomEntries { get; set; }
        public int? AppointmentPackageFixed { get; set; }
        public int? AppointmentUnidentifiedClients { get; set; }
        public int? AppointmentTax { get; set; }
        public int? AppointmentLeaveEntry { get; set; }
        public string InvoiceClient_MULTI { get; set; }
        public int? AppointmentBreakDuration { get; set; }
        public string AppointmentAccountsDefinitions { get; set; }
        public int? AppointmentDistanceVariance_STARTED { get; set; }
        public int? AppointmentDistanceVariance_COMPLETED { get; set; }
        public int? AppointmentSkipOnPublicHolidays { get; set; }
        public DateTime? AppointmentDateTimeAdded { get; set; }
        public Guid AppointmentUUID { get; set; }
        public float? AppointmentPaymentAmount { get; set; }
        public string AppointmentPaymentType { get; set; }
        public int? AppointmentGroupSession { get; set; }
    }

}
