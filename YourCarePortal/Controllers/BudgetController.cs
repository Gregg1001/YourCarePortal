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
using System.Security.Cryptography;
using System.Net.Http;

namespace YourCarePortal.Controllers
{
    public class BudgetController : Controller
    {
        private readonly ILogger<BudgetController> _logger;
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

        public BudgetController(
            ILogger<BudgetController> logger,
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
        /// Handles the Budget view request. This method retrieves the budget information
        /// and prepares the Budget view with required data.
        /// </summary>
        /// <remarks>
        /// This method performs the following:
        /// - Authenticates the user using an auth key from a cookie.
        /// - Conditionally checks for debug mode and initializes session data.
        /// - Fetches client data and prepares the Budget view if successful.
        /// - Handles the case where no data is available by redirecting to a NoData view.
        /// - Logs page access and handles white label functionality based on the current URL.
        /// </remarks>
        /// <returns>Returns an IActionResult that represents the result of the Budget operation.</returns>
        /// 
        /// <summary>
        /// Asynchronously handles the budget-related requests by checking user authentication,
        /// loading necessary data, and returning the appropriate view.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, resulting in an IActionResult.</returns>
        public async Task<IActionResult> Budget()
        {
            var authKey = string.Empty;
            var portalUserEmail = string.Empty;
            // Attempt to fetch  data using the Budget Pre Load Service
            var (WhiteLabelLocation, Authentication, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

            // If the cookie is missing, redirect to the Index view
            if (!Request.Cookies.ContainsKey("TP_YCP_1")){
                if (!string.IsNullOrEmpty(WhiteLabelLocation))
                {
                    ViewBag.WhiteLabel = WhiteLabelLocation;
                }
                TempData["ErrorMessage"] = "Email or Password incorrect. Please try again.";
                return View("Index");
            }

            try
            {
                // Safely attempt to deserialize the JSON string into a Cookie object
                // Attempt to retrieve the authentication key from the cookie, if not found, use an empty string
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                var myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
                authKey = myObject1.AuthKey ?? string.Empty;
                portalUserEmail = myObject1?.portalUserEmail ?? string.Empty;
            }
            catch (Exception ex)
            {
                //TempData["ErrorMessage"] = "Failed get Cookie data.";
                if (!string.IsNullOrEmpty(WhiteLabelLocation))
                {
                    ViewBag.WhiteLabel = WhiteLabelLocation;
                }
                return View("Index");
            }

            // If the authentication key is missing, set an error message and redirect to the Index view
            if (string.IsNullOrEmpty(authKey))
            {
                TempData["ErrorMessage"] = "Failed to Authenticate";
                if (!string.IsNullOrEmpty(WhiteLabelLocation)){
                    ViewBag.WhiteLabel = WhiteLabelLocation;}
                return View("Index");
            }

            // Attempt to fetch  data using the Budget Pre Load Service
            //var (WhiteLabelLocation, Authentication, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

            //If Portal Access is not authorised (can be due to password changed in TP!)
            //Go to the Index view (login page)
            if (Authentication.AuthorisationFailed)
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

            //set Menu options
            if (Authentication != null)
            {
                ViewBag.SETTING_StatementsEnabled = Authentication.SETTING_StatementsEnabled;
                ViewBag.SETTING_BudgetEnabled = Authentication.SETTING_BudgetEnabled;
                ViewBag.SETTING_SupportplanEnabled = Authentication.SETTING_SupportplanEnabled;
                ViewBag.SETTING_FormsEnabled = Authentication.SETTING_FormsEnabled;
                ViewBag.SETTING_NDIS_StatementsEnabled = Authentication.SETTING_NDIS_StatementsEnabled;
                ViewBag.SETTING_NDIS_Quotes_Enabled = Authentication.SETTING_NDIS_Quotes_Enabled;
            }
            if (!string.IsNullOrEmpty(WhiteLabelLocation))
            {
                ViewBag.WhiteLabel = WhiteLabelLocation;
            }

            // If no clients are found, display a message and redirect to a NoData view
            if (clients == null || !clients.Any())
            {
                ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                return View("NoData");
            }

            // Check if "Debug" parameter is present in the URL and set session flag accordingly
            if (Request.Query.ContainsKey("Debug"))
            {
                string debugValue = Request.Query["Debug"].ToString().ToLower();
                HttpContext.Session.SetString("YCP_Debug", debugValue == "true" ? "true" : "false");
            }

            // If the portal user email is missing, redirect to the Index view
            if (string.IsNullOrWhiteSpace(portalUserEmail))
            {
                return Redirect("/Home/Index");
            }

            //###
            if (Authentication.AuthorisationFailed)
            {
                Response.Cookies.Delete("TP_YCP_1");
                // Clear all session variables
                HttpContext.Session.Clear();
                if (!string.IsNullOrEmpty(WhiteLabelLocation)){
                    ViewBag.WhiteLabel = WhiteLabelLocation;}
                return View("Index");
            }

            // Initialize session data if not already set
            if (HttpContext.Session.GetString("portalUserID") == null)
            {
                var sessionManager = new SessionManager(_context, _responseHelper, _apiUrls, HttpContext, portalUserEmail);
                sessionManager.InitializeSession();
            }

            // Get client details and switch clients
            string client1 = clients.FirstOrDefault().clientID.ToString();
            var URL1 = _apiUrls.ClientDetails + "?clientID=" + client1;
            var (root1, debugMessage1, jsonString1) = await _responseHelper.GetResponse<Root>(authKey, URL1);

            // Fetch the budget data using the response helper
            var (root, debugMessage, jsonString) = await _responseHelper.GetResponse<Root>(authKey, _apiUrls.Budget);

            // If the budget data is null, set up the NoData view
            if (root == null)
            {
                ViewBag.NoDataMessage = _noDataMsg.BudgetMainText;
                ViewBag.NoDataSubText = _noDataMsg.BudgetSubText;
                return View("NoData");
            }

            //If there is an active session, retrieve client photo paths
            if (root.clientID_ACTIVE_SESSION != null)
            {
                var firstRecord = clients.FirstOrDefault();
                if (firstRecord.clientPhotoPath1 != null || firstRecord.clientPhotoPath1 != string.Empty) ;
                {
                    string encoded = Uri.EscapeDataString(firstRecord.clientPhotoPath1);
                    ViewBag.ClientPhotoPath1 = encoded;
                }
            }
            else
            {
                // The next line is commented out, but you would typically return the "NoData" view here
                ViewBag.NoDataMessage = _noDataMsg.BudgetMainText;
                ViewBag.NoDataSubText = _noDataMsg.BudgetSubText;
                return View("NoData");
            }

            // Set debug information in the ViewBag for the view to use if needed
            ViewBag.DebugMessage = debugMessage;
            ViewBag.JsonString = jsonString;

            //Log Page Access
            ILogAccess logger = new UpdatePortalAccessLog();
            LogAccessHelper logHelper = new LogAccessHelper(logger, HttpContext);
            logHelper.LogPageAccess("Budget");

            // Prepare the model for the view
            var model = new BudgetandClients
            {
                ClientDetails = clients,
                Budget = root
            };

            // Set the title for the view and return the Budget view with the model
            ViewBag.Title = "Budget - Your Care Portal";
            return View("Budget", model);
        }

        /// <summary>
        /// Handles the budget-related actions for a specific client.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>A view containing budget-related information.</returns>
        
        [HttpGet]
        public async Task<IActionResult> BudgetAction(int? clientId)
        {
            try
            {
                // Fetch and deserialize cookie
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                Cookie myObject1;

                try
                {
                    myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
                }
                catch (Exception ex)
                {
                    // Log this specific exception before proceeding
                    Console.WriteLine(ex);
                    return View("Error");
                }

                var authKey = myObject1.AuthKey ?? string.Empty;

                if (string.IsNullOrEmpty(authKey))
                {
                    TempData["ErrorMessage"] = "Failed to Authenticate";
                    return View("Index");
                }

                // Get client details and switch clients
                var URL1 = _apiUrls.ClientDetails + "?clientID=" + clientId.ToString();
                var (root1, debugMessage1, jsonString1) = await _responseHelper.GetResponse<Root>(authKey, URL1);

                if (root1 == null)
                {
                    return View("NoData");
                }

                // Attempt to fetch data using the Pre Load Service
                var (_, _, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

                if (clients == null || !clients.Any())
                {
                    ViewBag.NoDataMessage = _noDataMsg.MainText;
                    ViewBag.NoDataSubText = _noDataMsg.SubText;
                    return View("NoData");
                }

                // Get budget details
                var (RootBudget, debugMessage, jsonString) = await _responseHelper.GetResponse<Root>(authKey, _apiUrls.Budget);

                if (RootBudget == null || RootBudget.clientID_ACTIVE_SESSION == null)
                {
                    ViewBag.NoDataMessage = _noDataMsg.BudgetMainText;
                    ViewBag.NoDataSubText = _noDataMsg.BudgetSubText;
                }

                ViewBag.DebugMessage = debugMessage;
                ViewBag.JsonString = jsonString;

                var matchingClients = clients.Where(c => clientId.HasValue && c.clientID == clientId.Value).ToList();

                var partialModel = new BudgetandClients
                {
                    ClientDetails = matchingClients,
                    Budget = RootBudget
                };

                return PartialView("BudgetPartial", partialModel);
            }
            catch (Exception ex)
            {
                // A proper logging method should replace this
                Console.WriteLine(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
