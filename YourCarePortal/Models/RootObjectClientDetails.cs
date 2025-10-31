using System.Xml.Linq;

namespace YourCarePortal.Models
{
    public class RootObjectClientDetails
    {
        public string Status { get; set; }
        public string ShowMenu { get; set; }
        public string PageTitle { get; set; }
        public List<ClientDetailsElement> ClientDetailsElement { get; set; }

    }
}
