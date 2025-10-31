namespace YourCarePortal.Models
{
    public class BudgetandClients
    {
        public IEnumerable<QueryClientDetails>? ClientDetails { get; set; } // From QueryClientDetails? to IEnumerable<QueryClientDetails>?
        public Root? Budget { get; set; }
    }

}
