using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using YourCarePortal.Models;
using YourCarePortal.Services;

namespace YourCarePortal.Services
{
    public class SessionPreLoadDataService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GetWhitelabelImageLocation _imageLocationService;
        private readonly AuthenticationHelper _authHelper;
        private readonly ClientDataHelperNew _clientDataHelper;

        /// <summary>
        /// Initializes a new instance of the BudgetDataService class.
        /// </summary>
        /// <param name="httpContextAccessor">Provides access to the HTTP context to manage sessions.</param>
        /// <param name="imageLocationService">The service to get white label image locations.</param>
        /// <param name="authHelper">The helper service for authentication tasks.</param>
        /// <param name="clientDataHelper">The helper service for fetching client data.</param>
        /// 
        public SessionPreLoadDataService(
            IHttpContextAccessor httpContextAccessor,
            GetWhitelabelImageLocation imageLocationService,
            AuthenticationHelper authHelper,
            ClientDataHelperNew clientDataHelper)
        {
            _httpContextAccessor = httpContextAccessor;
            _imageLocationService = imageLocationService;
            _authHelper = authHelper;
            _clientDataHelper = clientDataHelper;
        }

        /// <summary>
        /// Asynchronously gets the budget data, including white label location, authentication details,
        /// and client information. Utilizes session caching to minimize repeated data fetching.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a tuple with
        /// the white label location (string), authentication details (Authorisation), and a list of
        /// client details (IEnumerable&lt;QueryClientDetails&gt;).
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when authentication fails.</exception>
        public async Task<(string WhiteLabelLocation, Authorisation Authentication, IEnumerable<QueryClientDetails> Clients)> GetSessionPreLoadDataAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string whiteLabelLocation = string.Empty;
            Authorisation authentication = null;
            IEnumerable<QueryClientDetails> clients = null;

            // API Call 1: Authenticate
            string jsonString = httpContext.Request.Cookies["TP_YCP_1"] ?? string.Empty;
            var cookieData = JsonConvert.DeserializeObject<Cookie>(jsonString); // Assuming Cookie is a defined class
            var authKey = cookieData?.AuthKey ?? string.Empty;

            authentication = await _authHelper.Authenticate(authKey);
            httpContext.Session.SetString("AuthenticationData", JsonConvert.SerializeObject(authentication));

            if (string.IsNullOrEmpty(authKey))
            {
                // Handle failed authentication
                // ###
                // Note:This service cannot directly return a View or set TempData. 
                // Consider throwing an exception or handling this scenario differently.
                //throw new UnauthorizedAccessException("Failed to Authenticate");
                authentication.AuthorisationFailed = true;  
            }
           
            if (httpContext.Session.GetString("HasAPIData") != "true")
            {
                // Read URL 1: GetImageLocationFromUrl
                string currentUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
                whiteLabelLocation = _imageLocationService.GetImageLocationFromUrl(currentUrl);
                httpContext.Session.SetString("WhiteLabelLocation", whiteLabelLocation ?? "null");

                //// DB or API Call: GetClientByIds
                //string clientIDs = authentication.LinkedClientIDS ?? "0";
                //clients = await _clientDataHelper.GetClientByIds(clientIDs);
                //httpContext.Session.SetString("ClientsData", JsonConvert.SerializeObject(clients));

                // Indicate that API data has been fetched
                httpContext.Session.SetString("HasAPIData", "true");
            }
            else
            {
                // Retrieve the API data from session variables
                whiteLabelLocation = httpContext.Session.GetString("WhiteLabelLocation") ?? string.Empty;
                var authenticationData = httpContext.Session.GetString("AuthenticationData") ?? string.Empty;
                authentication = JsonConvert.DeserializeObject<Authorisation>(authenticationData);
                var clientsData = httpContext.Session.GetString("ClientsData") ?? string.Empty;
                clients = JsonConvert.DeserializeObject<IEnumerable<QueryClientDetails>>(clientsData);
            }


            //Later investigate YCP-142
            // DB or API Call: GetClientByIds
            string clientIDs = authentication.LinkedClientIDS ?? "0";
            clients = await _clientDataHelper.GetClientByIds(clientIDs);
            httpContext.Session.SetString("ClientsData", JsonConvert.SerializeObject(clients));

            return (whiteLabelLocation, authentication, clients);
        }
    }


}
