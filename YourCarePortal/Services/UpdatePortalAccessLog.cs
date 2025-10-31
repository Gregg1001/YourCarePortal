using YourCarePortal.Interfaces;
/// <summary>
/// Provides an implementation for logging user access within the portal.
/// </summary>
public class UpdatePortalAccessLog : ILogAccess
{
    /// <summary>
    /// Logs access to a page by a user at a specified time for a particular provider.
    /// </summary>
    /// <param name="pageName">The name of the page being accessed.</param>
    /// <param name="portalUserID">The user ID of the portal user accessing the page.</param>
    /// <param name="dateTime">The date and time when the access occurred.</param>
    /// <param name="providerID">The provider ID associated with the user access event.</param>
    public void LogAccess(string pageName, int portalUserID, DateTime dateTime, int providerID)
    {
        // Path to the Logs directory
        string logFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

        // Check if the Logs directory exists; if not, create it
        if (!Directory.Exists(logFolderPath))
        {
            Directory.CreateDirectory(logFolderPath);
        }

        // Generate the log file name using the current date to create daily log files
        string logFileName = $"{dateTime:yyyy-MM-dd}_Log.txt";
        string logFilePath = Path.Combine(logFolderPath, logFileName);

        // Create a log entry with the provided parameters in a comma-separated format
        // Including single quotes for the dateTime and pageName to ensure proper formatting in the log file
        string logEntry = $"'{dateTime:yyyy-MM-dd HH:mm:ss}','{pageName}',{portalUserID},{providerID}";

        // Append the log entry to the file, creating the file if it does not exist
        using (StreamWriter sw = File.AppendText(logFilePath))
        {
            sw.WriteLine(logEntry);
        }
    }
}
