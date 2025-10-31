namespace YourCarePortal.Models
{
    public class NDISQuotesandClients
    {
        public IEnumerable<QueryClientDetails>? ClientDetails { get; set; } 
        public RootNDISQuotes NDISQuotes { get; set; }
    }
}
