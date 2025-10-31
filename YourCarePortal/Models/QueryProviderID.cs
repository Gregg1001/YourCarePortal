using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{
    public class QueryProviderID
    {
        [Key]
        public int? clientCompanyID { get; set; }
    }
}
