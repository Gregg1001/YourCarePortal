namespace YourCarePortal.Models
{
    public class StatementDetailsandClients
    {
        public IEnumerable<QueryClientDetails>? ClientDetails { get; set; } // From QueryClientDetails? to IEnumerable<QueryClientDetails>?
        public RootStatementDetails? StatementDetails { get; set; }
    }
}
