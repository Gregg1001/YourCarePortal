using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{
    public partial class TblPortalUser
    {
        [Key]
        public int PortalUserId { get; set; }
        public string? PortalUserFullname { get; set; }
        public string? PortalUserFirstName { get; set; }
        public string? PortalUserSurname { get; set; }
        public string? PortalUserEmail { get; set; }
        public string? PortalUserPassword { get; set; }
        public string? PortalUserMobile { get; set; }
        public string? PortalUserRelationship { get; set; }
        public string? PortalUserNotes { get; set; }
        public string? PosrtUserPermissions { get; set; }
        public int? PortalUserClientId { get; set; }
        public DateTime? PortalUserLastUpdated { get; set; }
        public DateTime? PortalUserLastLogin { get; set; }
        public bool? PortalUserDeleted { get; set; }
        public int PortalUserCompany { get; set; }
        public string? PortalUserToken { get; set; }
        public DateTime? PortalUserLastNotification { get; set; }
        public int? PortalUserNotificationSettings { get; set; }
        public string? PortalUserClientsAdditionalIds { get; set; }
        public int? PortalUserClientIdActiveSession { get; set; }
    }
}
