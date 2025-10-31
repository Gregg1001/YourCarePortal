using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{
    public class QueryClient
    {
        [Key]
        public int clientID { get; set; }
        public string? clientFirstname { get; set; }
        public string? clientSurname { get; set; }
        public string? clientAddress { get; set; }
        public string? clientSuburb { get; set; }
        public string? clientState { get; set; }
        public string? clientPostcode { get; set; }
        public string? clientFullName { get; set; }
    }

}


