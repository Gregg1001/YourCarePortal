using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class Root
    {
        public string PageTitle { get; set; }
        public string BudgetBalance { get; set; }
        public string DateLocked { get; set; }
        public string clientFullname_ACTIVE_SESSION { get; set; }
        public string clientID_ACTIVE_SESSION { get; set; }
        public List<BudgetCategory> BudgetContents { get; set; }
    }
}