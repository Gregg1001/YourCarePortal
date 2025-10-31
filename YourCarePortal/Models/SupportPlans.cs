namespace YourCarePortal.Models
{
    public class SupportPlans
    {
        public List<SupportPlanPlanItem> Items { get; set; } = new List<SupportPlanPlanItem>();
        public List<SupportPlanGoalGroup> Goals { get; set; } = new List<SupportPlanGoalGroup>();
    }

}
