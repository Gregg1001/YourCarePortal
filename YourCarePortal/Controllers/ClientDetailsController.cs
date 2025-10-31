using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YourCarePortal.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using YourCarePortal.Data;
using YourCarePortal.Services;
using Microsoft.Extensions.Options;
using YourCarePortal.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace YourCarePortal.Controllers
{
    public class ClientDetailsController : Controller
    {
        private readonly ILogger<ClientDetailsController> _logger;
        private readonly AuthenticationHelper _authHelper;
        private readonly ChangePasswordHelper _changePasswordHelper;
        private readonly ResponseHelper _responseHelper;
        private readonly ApiUrls _apiUrls;
        private readonly NoDataMsg _noDataMsg;
        private readonly DatabaseContext _context;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _clientFactory;
        private readonly GetWhitelabelImageLocation _imageLocationService;
        private readonly IConfiguration _configuration;
        private readonly SessionPreLoadDataService _sessionPreLoadDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientDetailsController(
            ILogger<ClientDetailsController> logger,
            AuthenticationHelper authHelper,
            ChangePasswordHelper changePasswordHelper,
            ResponseHelper responseHelper,
            IOptions<ApiUrls> apiUrls,
            IOptions<NoDataMsg> noDataMsg,
            DatabaseContext context,
            IMemoryCache memoryCache,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            SessionPreLoadDataService sessionPreLoadDataService)
        {
            _logger = logger;
            _authHelper = authHelper;
            _changePasswordHelper = changePasswordHelper;
            _responseHelper = responseHelper;
            _apiUrls = apiUrls.Value;
            _noDataMsg = noDataMsg.Value;
            _context = context;
            _cache = memoryCache;
            _clientFactory = clientFactory;
            _imageLocationService = new GetWhitelabelImageLocation(configuration);
            _configuration = configuration;
            _sessionPreLoadDataService = sessionPreLoadDataService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Fetches and displays client details based on the authenticated user's permissions and associated client IDs.
        /// </summary>
        /// <returns>
        /// An IActionResult that represents the outcome of the client details retrieval operation.
        /// </returns>
        public async Task<IActionResult> ClientDetails()
        {
            // Attempt to fetch  data using the Budget Pre Load Service
            var (WhiteLabelLocation, Authentication, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

            // If the cookie is missing, redirect to the Index view
            if (!Request.Cookies.ContainsKey("TP_YCP_1"))
            {
                if (!string.IsNullOrEmpty(WhiteLabelLocation))
                {
                    ViewBag.WhiteLabel = WhiteLabelLocation;
                }
                TempData["ErrorMessage"] = "Email or Password incorrect. Please try again.";
                return View("Index");
            }

            // Retrieve the authentication key from the cookie
            string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;

            // Safely attempt to deserialize the JSON string into a Cookie object
            Cookie myObject1;
            try
            {
                myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to parse authentication data.";
                return View("Index");
            }

            // ###
            // if AUTHKey is null, return to the login page


            if (myObject1 == null || myObject1.AuthKey is null)
            {
                Response.Cookies.Delete("TP_YCP_1");
                // Clear all session variables
                HttpContext.Session.Clear();
                if (!string.IsNullOrEmpty(WhiteLabelLocation)){
                    ViewBag.WhiteLabel = WhiteLabelLocation;}
                return View("Index");
            }    

            var authKey = myObject1.AuthKey ?? string.Empty;

            // Validate the authentication key
            if (string.IsNullOrEmpty(authKey))
            {
                TempData["ErrorMessage"] = "Failed to Authenticate";
                return View("Index");
            }

            // Get Email from Cookie
            string jsonString31 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
            var myObject11 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
            var portalUserEmail = myObject11?.portalUserEmail ?? string.Empty;
            string? email = portalUserEmail;

            // InitializeSession Data
            if (HttpContext.Session.GetString("portalUserID") == null)
            {
                var sessionManager = new SessionManager(_context, _responseHelper, _apiUrls, HttpContext, email);
                sessionManager.InitializeSession();
            }

            //If Portal Access is not authorised (can be due to password changed in TP!
            //Go to the Index view (login page)
            if (Authentication.AuthorisationFailed)
            {
                Response.Cookies.Delete("TP_YCP_1");
                // Clear all session variables
                HttpContext.Session.Clear();
                if (!string.IsNullOrEmpty(WhiteLabelLocation)){
                    ViewBag.WhiteLabel = WhiteLabelLocation;}
                return View("Index");
            }

            if (Authentication != null)
            {
                ViewBag.SETTING_StatementsEnabled = Authentication.SETTING_StatementsEnabled;
                ViewBag.SETTING_BudgetEnabled = Authentication.SETTING_BudgetEnabled;
                ViewBag.SETTING_SupportplanEnabled = Authentication.SETTING_SupportplanEnabled;
                ViewBag.SETTING_FormsEnabled = Authentication.SETTING_FormsEnabled;
                ViewBag.SETTING_NDIS_StatementsEnabled = Authentication.SETTING_NDIS_StatementsEnabled;
                ViewBag.SETTING_NDIS_Quotes_Enabled = Authentication.SETTING_NDIS_Quotes_Enabled;

                // Store the linked client IDs in the session for future use
                HttpContext.Session.SetString("PortalUserClientsAdditionalIds", Authentication?.LinkedClientIDS ?? "null");
            }

            if (WhiteLabelLocation != null)
            {
                ViewBag.WhiteLabel = WhiteLabelLocation;
            }

            if (clients != null && clients.Any())
            {
                // Retrieve the associated client IDs from the session
                string ClientIDs = HttpContext.Session.GetString("PortalUserClientsAdditionalIds") ?? "0";

                // If client IDs are available, fetch the client details
                if (!string.IsNullOrEmpty(ClientIDs))
                {

                    if (clients == null || !clients.Any())
                    {
                        ViewBag.NoDataMessage = _noDataMsg.ClientDetailsMainText;
                        ViewBag.NoDataSubText = _noDataMsg.ClientDetailsSubText;

                        return View("NoData");
                    }

                    // Cache the fetched client details for potential future use
                    _cache.Set("ClientDetailsRS1", clients);

                    // Set the page title and display the client details
                    ViewBag.Title = "Client Details - Your Care Portal";

                    //Log Page Access
                    ILogAccess logger = new UpdatePortalAccessLog();
                    LogAccessHelper logHelper = new LogAccessHelper(logger, HttpContext);
                    logHelper.LogPageAccess("ClientDetails");

                    string sqlClient = @"SELECT [clientID], [clientFirstname], [clientSurname], [clientAddress], [clientSuburb],[clientState], [clientPostcode], [clientFullName], [clientCaseManager], uS.[userFirstName], US.[userSurname],[ClientCompanyID]
                                , [companyName], [companyPhone], [companyAddress], [companySuburb], [companyPostcode], [companyState], [companyLogo], [clientPhotoPath1]
                                FROM[tblClients] cl
                                INNER JOIN[dbo].[tblCompanies] co ON cl.[clientCompanyID] = co.[companyID]
                                LEFT JOIN [dbo].[tblUsers] us ON cl.[clientCaseManager] = [userID]
                                WHERE [clientID] IN ({0});";

                    ViewBag.DebugMessage = "SQL:";
                    ViewBag.JsonString = sqlClient;

                    return View("ClientDetails", clients);
                }

                ViewBag.NoDataMessage = _noDataMsg.ClientDetailsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientDetailsSubText;
                return View("NoData");
            }

            ViewBag.NoDataMessage = _noDataMsg.ClientDetailsMainText;
            ViewBag.NoDataSubText = _noDataMsg.ClientDetailsSubText;
            return View("NoData");
        }

        /// <summary>
        /// Asynchronous action method to fetch client details based on optional client ID.
        /// </summary>
        /// <param name="clientId">Optional client ID to filter the details.</param>
        /// <returns>IActionResult containing either client details or an error message.</returns>
        ///
        [HttpGet]
        public async Task<IActionResult> ClientDetailsAction(int? clientId)
        {
            try
            {
                // Retrieve the cookie data
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                Cookie myObject1 = null;

                // Try to deserialize the JSON string from the cookie
                try
                {
                    myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
                }
                catch (Exception ex)
                {
                    // Log the JSON deserialization error for debugging purposes
                    Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                    // Return an error view indicating that the cookie data is invalid
                    return View("Error", new { message = "Invalid Cookie Data" });
                }

                // Retrieve authentication key from deserialized object
                var authKey = myObject1?.AuthKey ?? string.Empty;

                // Check if authentication key is present
                if (string.IsNullOrEmpty(authKey))
                {
                    TempData["ErrorMessage"] = "Failed to Authenticate";
                    return View("Index");
                }

                // Attempt to fetch data using the Pre Load Service
                var (_, _, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

                // Check if the helper was successfully initialized
                if (clients != null)
                {
                    string ClientIDs = HttpContext.Session.GetString("PortalUserClientsAdditionalIds") ?? "0";

                    if (!string.IsNullOrEmpty(ClientIDs))
                    {

                        // If client ID is not specified, return all clients
                        if (!clientId.HasValue)
                        {
                            return PartialView("ClientDetailsPartial", clients);
                        }

                        // Build the API URL for fetching details of the specific client
                        var URL1 = _apiUrls.ClientDetails + "?clientID=" + clientId.ToString();

                        // Fetch client details from API
                        var (root, debugMessage, jsonString) = await _responseHelper.GetResponse<Root>(authKey, URL1);

                        // Check if the API returned valid data
                        if (root == null)
                        {
                            return View("NoData");
                        }

                        // Filter clients that match the provided client ID
                        var matchingClients = clients.Where(c => c.clientID == clientId.Value).ToList();
                        return PartialView("ClientDetailsPartial", matchingClients);
                    }

                    // If we reached this point, something went wrong.
                    return View("Error", new { message = "Unexpected error." });
                }
                else
                {
                    return View("Error", new { message = "Helper is not initialized." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Console.WriteLine(ex);

                // Return a JSON object with error information
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
