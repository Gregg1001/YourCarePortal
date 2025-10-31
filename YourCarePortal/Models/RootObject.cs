namespace YourCarePortal.Models
{
    public class RootObject
    {
        public List<AppointmentsFound> AppointmentsFound { get; set; }
        public List<Schedule> Appointments { get; set; }
        public List<PageLoad> PageLoad { get; set; }
        public string TodayDate { get; set; }
        public string TimeNow { get; set; }
    }
}
