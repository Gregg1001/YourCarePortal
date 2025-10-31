namespace YourCarePortal.Models
{
    public class SupportPlanGoalGroup
    {
        public SupportPlanPlanItem Goal { get; set; }
        public List<SupportPlanPlanItem> OtherItems { get; set; } = new List<SupportPlanPlanItem>();
        public List<SupportPlanActionGroup> Actions { get; set; } = new List<SupportPlanActionGroup>();
    }
}
