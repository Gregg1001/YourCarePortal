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
    public class AppointmentsController : Controller
    {
        private readonly ILogger<StatementsController> _logger;
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

        public AppointmentsController(
            ILogger<StatementsController> logger,
            AuthenticationHelper authHelper,
            ChangePasswordHelper changePasswordHelper,
            ResponseHelper responseHelper,
            IOptions<ApiUrls> apiUrls,
            IOptions<NoDataMsg> noDataMsg,
            DatabaseContext context,
            IMemoryCache memoryCache,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            SessionPreLoadDataService sessionPreLoadDataService,
            IHttpContextAccessor httpContextAccessor)

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
        /// Asynchronously processes the appointment page request, handling user authentication,
        /// session initialization, and data retrieval for appointments.
        /// </summary>
        /// <param name="email">The email of the user for authentication.</param>
        /// <param name="password">The password of the user for authentication.</param>
        /// <returns>A task that represents the asynchronous operation, resulting in an IActionResult for the appointments page.</returns>
        public async Task<IActionResult> Appointments(string email, string password,string viewname)
        {
             //1
            //White Label//
            // Obtain the current URL
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            // Fetch the image location using the service
            var WhiteLabelLocation = _imageLocationService.GetImageLocationFromUrl(currentUrl);

            // Not needed
            // If the cookie is missing, redirect to the Index view
            //if (!Request.Cookies.ContainsKey("TP_YCP_1"))
            //{
            //    if (!string.IsNullOrEmpty(WhiteLabelLocation))
            //    {
            //        ViewBag.WhiteLabel = WhiteLabelLocation;
            //    }
            //    TempData["ErrorMessage"] = "Email or Password incorrect. Please try again.";
            //    return View("Index");
            //}

            if (WhiteLabelLocation != null)
            {
                ViewBag.WhiteLabel = WhiteLabelLocation;
            }

            // Attempt to fetch  data using the Budget Pre Load Service
            // TBC
            //var (WhiteLabelLocation, _, _) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

            // If the email is provided, set the user's email in the session for later use
            if (email != null)
            {
                HttpContext.Session.SetString("portalUserEmail", email);
            }

            // Check if "Debug" parameter is present in the URL and set session flag accordingly
            HttpContext.Session.SetString("DebugFlag",
                Request.Query.ContainsKey("Debug") && Request.Query["Debug"].ToString().ToLower() == "true" ? "true" : "false");

            // Variable to store the authentication key
            string authKey;

            // Attempt to authenticate the user with an authentication key if present in cookies
            if (Request.Cookies.ContainsKey("TP_YCP_1"))
            {
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                var myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);

                authKey = myObject1.AuthKey ?? string.Empty;

                //2
                // Proceed with authentication using the helper and update the session and ViewBag with settings
                var Authentication1 = await _authHelper.Authenticate(authKey);

                if (Authentication1.AuthorisationFailed)
                {
                    Response.Cookies.Delete("TP_YCP_1");
                    // Clear all session variables
                    HttpContext.Session.Clear();
                    if (!string.IsNullOrEmpty(WhiteLabelLocation)){
                        ViewBag.WhiteLabel = WhiteLabelLocation;}
                    TempData["ErrorMessage"] = "Email or Password incorrect. Please try again.";
                    return View("Index");
                }

                if (Authentication1 != null)
                {
                    // Populate settings into ViewBag for use within the view
                    ViewBag.SETTING_StatementsEnabled = Authentication1.SETTING_StatementsEnabled;
                    ViewBag.SETTING_BudgetEnabled = Authentication1.SETTING_BudgetEnabled;
                    ViewBag.SETTING_SupportplanEnabled = Authentication1.SETTING_SupportplanEnabled;
                    ViewBag.SETTING_FormsEnabled = Authentication1.SETTING_FormsEnabled;
                    ViewBag.SETTING_NDIS_StatementsEnabled = Authentication1.SETTING_NDIS_StatementsEnabled;
                    ViewBag.SETTING_NDIS_Quotes_Enabled = Authentication1.SETTING_NDIS_Quotes_Enabled;

                    // Store additional client IDs into the session
                    HttpContext.Session.SetString("PortalUserClientsAdditionalIds", Authentication1?.LinkedClientIDS ?? "null");
                    TempData["PortalUserClientsAdditionalIds"] = Authentication1?.LinkedClientIDS ?? "null";
                }
            }
            // If email and password are provided, attempt to authenticate the user using these credentials
            else if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var Authentication1 = new Authorisation();

                try
                {
                    Authentication1 = await _authHelper.Authenticate(email, password);

                    // Handle the authentication result
                    if (Authentication1 != null)
                    {
                        if (Authentication1.AuthorisationFailed)
                        {
                            Response.Cookies.Delete("TP_YCP_1");
                            // Clear all session variables
                            HttpContext.Session.Clear();
                            if (!string.IsNullOrEmpty(WhiteLabelLocation)){
                                ViewBag.WhiteLabel = WhiteLabelLocation;}
                            TempData["ErrorMessage"] = "Email or Password incorrect. Please try again.";
                            return View("Index");
                        }
                    }
                    else
                    {
                       // Handle the case where authenticationResult is null
                    }

                    //return View("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Email or Password incorrect. Please try again.";

                    return View("Index");
                }

                // Save the authentication key from the response into the session
                authKey = Authentication1.AUTHKEY;

                // Create a cookie with authentication details if email was provided during authentication
                if (email != null)
                {
                    CookieOptions options2 = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(2)
                    };

                    // Serialize the authentication object into a JSON string and create the cookie
                    string jsonString1 = JsonConvert.SerializeObject(new
                    {
                        AuthKey = Authentication1.AUTHKEY,
                        PortalUserEmail = email,
                        // More properties can be added here as needed
                    });
                    Response.Cookies.Append("TP_YCP_1", jsonString1, options2);

                    // Populate settings into ViewBag for use within the view
                    ViewBag.SETTING_StatementsEnabled = Authentication1.SETTING_StatementsEnabled;
                    ViewBag.SETTING_BudgetEnabled = Authentication1.SETTING_BudgetEnabled;
                    ViewBag.SETTING_SupportplanEnabled = Authentication1.SETTING_SupportplanEnabled;
                    ViewBag.SETTING_FormsEnabled = Authentication1.SETTING_FormsEnabled;
                    ViewBag.SETTING_NDIS_StatementsEnabled = Authentication1.SETTING_NDIS_StatementsEnabled;
                    ViewBag.SETTING_NDIS_Quotes_Enabled = Authentication1.SETTING_NDIS_Quotes_Enabled;

                    TempData["PortalUserClientsAdditionalIds"] = Authentication1?.LinkedClientIDS ?? "null";

                    if (Authentication1.AuthorisationFailed)
                    {
                        Response.Cookies.Delete("TP_YCP_1");
                        // Clear all session variables
                        HttpContext.Session.Clear();
                        if (!string.IsNullOrEmpty(WhiteLabelLocation))
                        {
                            ViewBag.WhiteLabel = WhiteLabelLocation;
                        }
                        TempData["ErrorMessage"] = "Email or Password incorrect. Please try again.";
                        return View("Index");
                    }
                }
            }
            else
            {
                // If no authentication key, email, or password is provided, consider it as a white label scenario and return to the Index view
                return View("Index");
            }

            //###
            //Initialize session data if the portal user ID is not set in the session
            if (HttpContext.Session.GetString("portalUserID") == null)
            {
                var sessionManager = new SessionManager(_context, _responseHelper, _apiUrls, HttpContext, email);
                sessionManager.InitializeSession();
            }

            //3
            // Retrieve client IDs from the session, which are necessary for loading client-specific data
            string clientIDs = HttpContext.Session.GetString("PortalUserClientsAdditionalIds") ?? "0";

            //If no client IDs are found, check if they are available in TempData
            if (clientIDs == "0")
            {
                clientIDs = TempData["PortalUserClientsAdditionalIds"] as string;
            }

            if (string.IsNullOrEmpty(clientIDs))
            {
                // If no client IDs are found, return the NoData view with an appropriate message
                ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                return View("NoData");
            }

            //3.1
            // Use the client data helper to fetch the client details
            var helper1 = new ClientDataHelper(_context, _responseHelper, _apiUrls, HttpContext);
            var clients = await helper1.GetClientByIds(clientIDs);

            // If no client data is found, return the NoData view with a message
            if (clients == null || !clients.Any())
            {
                ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                return View("NoData");
            }
    
            // Retrieve appointment data using the response helper
            var (rootObject, debugMessage, jsonString) = await _responseHelper.GetResponse<RootObject>(authKey, _apiUrls.Schedule);

            // Cache the root object if needed for later use
            _cache.Set("RootObjectKey", rootObject);

            // Prepare the model for the view with client details and appointment data
            var model = new ScheduleandClients
            {
                ClientDetails = clients,
                Appointments = rootObject.Appointments
            };

            //Override/Redirect the View - NDIS quotes
            if ((viewname != null) && (viewname.Contains("NDISQuotes")))
            {
                HttpContext.Response.Redirect("/NDISQuotes/" + viewname);
             
            }

            // Log page access and white label processing based on the current URL
            //string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            ILogAccess logger1 = new UpdatePortalAccessLog();
            LogAccessHelper logHelper1 = new LogAccessHelper(logger1, HttpContext);
            logHelper1.LogPageAccess("Schedule");


            // Set the title for the view and return the Appointments view with the prepared model
            ViewBag.Title = "Schedule - Your Care Portal";
            return View("Appointments", model);
        }

        /// <summary>
        /// Handles the HTTP GET request to filter appointments by client ID and date range.
        /// </summary>
        /// <param name="startDate">The start date for filtering appointments.</param>
        /// <param name="endDate">The end date for filtering appointments.</param>
        /// <param name="clientId">The client ID for filtering appointments.</param>
        /// <returns>A PartialView containing the list of filtered appointments.</returns>
        
        [HttpGet]

        public IActionResult AppointmentsAction(DateTime? startDate, DateTime? endDate, int? clientId)
        {

            RootObject rootObject;

            // Check if the RootObject is in cache
            if (!_cache.TryGetValue("RootObjectKey", out rootObject))
            {
                // TODO: Handle scenarios when the cache is empty
                // For example, fetch data from another source or return an appropriate response.
            }

            // Start with all appointments; ensuring it's enumerable for LINQ operations
            var filteredAppointments = rootObject.Appointments.AsEnumerable();

            // Filter by Client ID if present
            if (clientId.HasValue)
            {
                var clientIdStr = clientId.Value.ToString();
                filteredAppointments = filteredAppointments.Where(a => a.ClientID == clientIdStr);
            }

            // Filter by Date Range if both startDate and endDate are provided
            if (startDate.HasValue && endDate.HasValue)
            {
                filteredAppointments = filteredAppointments.Where(a =>
                {
                    DateTime appointmentDate;
                    bool isParsed = DateTime.TryParse(a.Date, out appointmentDate);
                    return isParsed && appointmentDate >= startDate.Value && appointmentDate <= endDate.Value;
                });
            }

            // Initialize Viewbag properties for handling 'No Data' scenarios
            ViewBag.NoDataMessage = _noDataMsg.ApptsMainText;
            ViewBag.NoDataSubText = _noDataMsg.ApptsSubText;
            ViewBag.IsPartialNull = true;

            // Prepare the model for the PartialView
            var model = filteredAppointments.ToList();

            return PartialView("SchedulePartial", model);
        }
    }
}
