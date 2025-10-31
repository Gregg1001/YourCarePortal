namespace YourCarePortal.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class ClientService
    {
        public async Task<int> GetFirstClientId(string clientIDs)
        {
            int firstClientId = 0; // Default value if no clientIDs are provided

            if (!string.IsNullOrEmpty(clientIDs))
            {
                var clientIdsArray = clientIDs.Split(',');
                if (clientIdsArray.Length > 0)
                {
                    if (!int.TryParse(clientIdsArray[0], out firstClientId))
                    {
                        // Handle the case where the first value in the CSV is not a number
                        // You may throw an exception, log an error, or handle it according to your application's logic
                        throw new ArgumentException("First value in the CSV is not a valid number.", nameof(clientIDs));
                    }
                }
            }

            return firstClientId;
        }
    }

}
