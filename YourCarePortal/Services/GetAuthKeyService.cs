using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using YourCarePortal.Models; // Replace with the correct namespace if Cookie model is in a different namespace.

//not used 16/1/2024

namespace YourCarePortal.Services
{


    public class GetAuthKeyService
    {
        public GetAuthKeyService()
        {
            // Constructor logic if needed
        }

        /// <summary>
        /// Retrieves the authentication key from the HTTP request cookie.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The authentication key, or an empty string if not found.</returns>
        public async Task<string> GetAuthKeyFromRequestAsync(HttpRequest request)
        {
            // Assuming the cookie name is "TP_YCP_1"
            string cookieName = "TP_YCP_1";
            string authKey = string.Empty;

            if (request.Cookies.ContainsKey(cookieName))
            {
                string cookieValue = request.Cookies[cookieName];

                // Deserialize the cookie value. Assuming 'Cookie' is a class that has 'AuthKey' property.
                var cookieObject = JsonConvert.DeserializeObject<Cookie>(cookieValue);
                authKey = cookieObject?.AuthKey ?? string.Empty;
            }

            return authKey;
        }
    }
}
