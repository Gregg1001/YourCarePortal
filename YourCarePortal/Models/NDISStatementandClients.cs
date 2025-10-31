namespace YourCarePortal.Models
{
    public class NDISStatementandClients
    {
        public IEnumerable<QueryClientDetails>? ClientDetails { get; set; } // From QueryClientDetails? to IEnumerable<QueryClientDetails>?
        public RootNDISStatement? NDISStatement { get; set; }
    }
}