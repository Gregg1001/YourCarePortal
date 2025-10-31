namespace YourCarePortal.Models
{
    public class StatementsandClients
    {
        public IEnumerable<QueryClientDetails>? ClientDetails { get; set; } // From QueryClientDetails? to IEnumerable<QueryClientDetails>?
        public RootStatements? Statements { get; set; }

    }
}

