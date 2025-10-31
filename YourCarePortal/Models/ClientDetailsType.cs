using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{
    public class ClientDetailsType
    {
        [Key]
        public int clientID { get; set; }
        public string clientFirstname { get; set; }
        public string clientSurname { get; set; }
        public string clientAddress { get; set; }
        public string clientSuburb { get; set; }
        public string clientState { get; set; }
        public string clientPostcode { get; set; }
        public string clientFullName { get; set; }
        public int? clientCaseManager { get; set; }
        public string? userFirstName { get; set; }
        public string? userSurname { get; set; }
        public int? clientCompanyID { get; set; }
        public string companyName { get; set; }
        public string companyPhone { get; set; }
        public string companyAddress { get; set; }
        public string companySuburb { get; set; }
        public string companyPostcode { get; set; }
        public string companyState { get; set; }
        public string companyLogo { get; set; }
    }
}
