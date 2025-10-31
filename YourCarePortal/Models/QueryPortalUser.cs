using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{
    public class QueryPortalUser
    {
        [Key]
        public int portalUserID { get; set; }
        public string? portalUserFullname { get; set; }
        public string? portalUserFirstname { get; set; }
        public string? portalUserSurname { get; set; }
        public int portalUserClientID { get; set; }
        public string? portalUserEmail { get; set; }
        public string? PortalUserClientsAdditionalIds { get; set; }
        public int portalUserCompany { get; set; }
    }
   
}


