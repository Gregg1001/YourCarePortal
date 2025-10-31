namespace YourCarePortal.Services
{
    public class DateService
    {
        public string GetPreviousMonth()
        {
            // Step 1: Get the current date
            DateTime currentDate = DateTime.Now;

            // Step 2: Calculate the previous month
            DateTime previousMonth = currentDate.AddMonths(-1);

            // Step 3: Format and return the previous month in "Month Year" format
            return previousMonth.ToString("MMMM yyyy");
        }
    }
}
