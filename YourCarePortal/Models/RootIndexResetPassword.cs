//using Newtonsoft.Json;

namespace YourCarePortal.Models
{
    public class RootIndexResetPassword
    {
        //[JsonProperty("status")]
        public string status { get; set; }

        //[JsonProperty("showMenu")]
        public string showMenu { get; set; }

        //[JsonProperty("pageTitle")]
        public string pageTitle { get; set; }

        //[JsonProperty("message")]
        public string message { get; set; }

        //[JsonProperty("tempCode")]
        public string tempCode { get; set; }

        //[JsonProperty("tempCode_hidden")]
        public string tempCode_hidden { get; set; }

       //[JsonProperty("email")]
        public string email { get; set; }

        //[JsonProperty("CurrentStep")]
        public string CurrentStep { get; set; }

        public string ErrorMessage { get; set; }

        public string newPassword { get; set; }
    }
}
