namespace YourCarePortal.Models
{
    public class OtherURLs
    {
         public string profilePics { get; set; }
         public string noProfilePic { get; set; }

        public static implicit operator List<object>(OtherURLs v)
        {
            throw new NotImplementedException();
        }
    }
}
