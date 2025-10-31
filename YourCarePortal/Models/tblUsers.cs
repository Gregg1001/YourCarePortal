using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{

    public class tblUsers
    {
        [Key]
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string UserTitle { get; set; }
        public string UserFirstName { get; set; }
        public string UserSurname { get; set; }
        public string UserFullName { get; set; }
        public int? UserGender { get; set; }
        public string UserDescription { get; set; }
        public int? UserCompanyId { get; set; }
        public int? UserDepartment { get; set; }
        public int? UserLevel { get; set; }
        public int? UserManagerId { get; set; }
        public DateTime? UserDOB { get; set; }
        public string UserAddress1 { get; set; }
        public string UserAddress2 { get; set; }
        public string UserCoordinates { get; set; }
        public string UserPostcode { get; set; }
        public string UserState { get; set; }
        public string UserPhone { get; set; }
        public string UserMobile { get; set; }
        public string UserAvailability { get; set; }
        public string UserQualifications { get; set; }
        public int? UserPayLevel { get; set; }
        public string UserAccountingSystemRef { get; set; }
        public string UserPayCustom { get; set; }
        public string UserExclusions1 { get; set; }
        public bool? UserNotifySms { get; set; }
        public bool? UserNotifyEmail { get; set; }
        public DateTime? UserDateModified { get; set; }
        public DateTime? UserDateLastLogin { get; set; }
        public bool? UserCareWorker { get; set; }
        public bool? UserDisabled { get; set; }
        public string UserQualificationsExp { get; set; }
        public string UserAvailabilityFt { get; set; }
        public bool? UserAccountsAccessEnabled { get; set; }
        public DateTime? UserLastChangeNotificationTime { get; set; }
        public bool? UserNotifyRosterNotifications { get; set; }
        public string UserLinkedAccounts { get; set; }
        public bool? UserEnableGpsTracking { get; set; }
        public string UserEmergencyContact { get; set; }
        public string UserEmergencyContactPhone { get; set; }
        public string UserLanguages { get; set; }
        public bool? UserMobilePinRequired { get; set; }
        public string UserMobilePin { get; set; }
        public int? UserDriver { get; set; }
        public int? UserTravelPay { get; set; }
        public int? UserHoursMax { get; set; }
        public int? UserHoursMin { get; set; }
        public string UserPrefName { get; set; }
        public bool? UserCaseManagerBln { get; set; }
        public int? UserPayGroupId { get; set; }
        public string UserExtraSettings1 { get; set; }
        public string UserRosteringNotes { get; set; }
        public bool? UserIgnoreConflicts { get; set; }
        public string UserAllowances { get; set; }
        public DateTime? UserDateEmploymentStart { get; set; }
        public DateTime? UserDateEmploymentEnd { get; set; }
        public bool? UserUnlockingPermissions { get; set; }
        public string UserWorksites { get; set; }
        public string UserPhotoPath1 { get; set; }
        public string UserNotificationsToken { get; set; }
        public string UserAccessClientCustomOption1 { get; set; }
        public int? UserRegion { get; set; }
        public int? UserPendingUpdatesCounter { get; set; }
        public string UserCustomSettings { get; set; }
        public string UserRegionsAccess { get; set; }
        public string UserAbn { get; set; }
        public DateTime? UserDatePasswordUpdated { get; set; }
        public Guid? UserUuid { get; set; }
        public string UserAvailabilitySplit { get; set; }
        public int? UserIgnoreAwardAlerts { get; set; }
        public string UserExternalAuth { get; set; }
    }

}
