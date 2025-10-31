namespace YourCarePortal.Models
{
    public class BudgetCategory
    {
        public string BudgetCategoryHeading { get; set; }
        public string BudgetCategoryTotal { get; set; }
        public List<BudgetContent> BudgetCategoryContent { get; set; }
    }
}
