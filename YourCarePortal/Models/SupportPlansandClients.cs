namespace YourCarePortal.Models
{
    public class SupportPlansandClients
    {
        public IEnumerable<QueryClientDetails>? ClientDetails { get; set; } // From QueryClientDetails? to IEnumerable<QueryClientDetails>?
        public SupportPlans? SupportPlans { get; set; }
    }
}
