namespace YourCarePortal.Interfaces
{
    public interface ILogAccess
    {
        void LogAccess(string pageName, int portalUserID, DateTime dateTime, int providerID);
        
    }
}
