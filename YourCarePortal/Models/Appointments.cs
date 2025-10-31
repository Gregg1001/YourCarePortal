namespace YourCarePortal.Models
{
    public class Appointments
    {
        public int Id { get; set; }
        public string ClientID { get; set; }
        public string ClientFullName { get; set; }
        public string Date { get; set; }
        public int Duration { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string UserID { get; set; }
        public string CareWorker { get; set; }
        public string ServiceTypeID { get; set; }
        public string ServiceTypeName { get; set; }
        public string PackageTypeID { get; set; }
        public string PackageTypeName { get; set; }
        public string LastModified { get; set; }
    }
}
