using System.ComponentModel.DataAnnotations;

namespace YourCarePortal.Models
{
    public class Cookie
    {
        [Key]
        public string? AuthKey { get; set; }
        public string? portalUserEmail { get; set; }
    }
}


