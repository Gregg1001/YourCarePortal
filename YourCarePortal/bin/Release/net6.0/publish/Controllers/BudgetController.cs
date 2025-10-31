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
            IConfiguration configuration)
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

            //White Label//
            // Obtain the current URL
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";

            // Fetch the image location using the service
            var WhiteLabelLocation = _imageLocationService.GetImageLocationFromUrl(currentUrl);

            if (WhiteLabelLocation != null)
            {
                ViewBag.WhiteLabel = WhiteLabelLocation;
            }

            //Log Page Access
            ILogAccess logger = new UpdatePortalAccessLog();
            LogAccessHelper logHelper = new LogAccessHelper(logger, HttpContext);
            logHelper.LogPageAccess("Budget");


            // Attempt to retrieve the authentication key from the cookie, if not found, use an empty string
            string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
            var myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);

            // Safely extract the AuthKey from the deserialized cookie object
            var authKey = myObject1.AuthKey ?? string.Empty;

            // If the authentication key is missing, set an error message and redirect to the Index view
            if (string.IsNullOrEmpty(authKey))
            {
                TempData["ErrorMessage"] = "Failed to Authenticate";
                return View("Index");
            }

            
            
            // Authenticate the user with the provided auth key
            var Authentication1 = await _authHelper.Authenticate(authKey);

            // If authentication is successful, store settings in the ViewBag for use in the view
            if (Authentication1 != null)
            {
                ViewBag.SETTING_StatementsEnabled = Authentication1.SETTING_StatementsEnabled;
                ViewBag.SETTING_BudgetEnabled = Authentication1.SETTING_BudgetEnabled;
                ViewBag.SETTING_SupportplanEnabled = Authentication1.SETTING_SupportplanEnabled;

                // Store additional client IDs in the session
                HttpContext.Session.SetString("PortalUserClientsAdditionalIds", Authentication1?.LinkedClientIDS ?? "null");

                // If the Budget feature is disabled, display a message and redirect to a NoData view
                if (Authentication1.SETTING_BudgetEnabled == "no")
                {
                    ViewBag.NoDataMessage = _noDataMsg.BudgetMainText;
                    ViewBag.NoDataSubText = _noDataMsg.BudgetSubText;

                    return View("NoData");
                }
            }


            // Check if "Debug" parameter is present in the URL and set session flag accordingly
            if (Request.Query.ContainsKey("Debug"))
            {
                string debugValue = Request.Query["Debug"].ToString().ToLower();
                HttpContext.Session.SetString("YCP_Debug", debugValue == "true" ? "true" : "false");
            }



            // Retrieve the user's email from the cookie to initialize session data
            string jsonString31 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
            var myObject11 = JsonConvert.DeserializeObject<Cookie>(jsonString31);
            var portalUserEmail = myObject11?.portalUserEmail ?? string.Empty;

            // Initialize session data if not already set
            if (HttpContext.Session.GetString("portalUserID") == null)
            {
                var sessionManager = new SessionManager(_context, _responseHelper, _apiUrls, HttpContext, portalUserEmail);
                sessionManager.InitializeSession();
            
            }

            // Retrieve client IDs from the session
            string clientIDs = HttpContext.Session.GetString("PortalUserClientsAdditionalIds") ?? "0";

            // If client IDs are missing, display a message and redirect to a NoData view
            if (string.IsNullOrEmpty(clientIDs))
            {
                ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                return View("NoData");
            }

            // Helper for retrieving client data
            var helper = new ClientDataHelper(_context, _responseHelper, _apiUrls, HttpContext);

            // Check if the helper is properly initialized before proceeding
            if (!helper.IsInitialized)
            {
                ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                return View("NoData");
            }

            // Fetch client data using the helper
            var clients = await helper.GetClientByIds(clientIDs);

            // If no clients are found, display a message and redirect to a NoData view
            if (clients == null || !clients.Any())
            {
                ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                return View("NoData");
            }


            // Fetch the budget data using the response helper
            var (root, debugMessage, jsonString) = await _responseHelper.GetResponse<Root>(authKey, _apiUrls.Budget);

            // If the budget data is null, set up the NoData view
            if (root == null)
            {
                ViewBag.NoDataMessage = _noDataMsg.BudgetMainText;
                ViewBag.NoDataSubText = _noDataMsg.BudgetSubText;
                // The next line is commented out, but you would typically return the "NoData" view here
                // return View("NoData");
            }


            //Get Clients

            // If the budget data is not null, process it further
            if (root != null)
            {
                // If there is an active session, retrieve client photo paths
                if (root.clientID_ACTIVE_SESSION != null)
                {
                    var helper1 = new ClientDataHelper(_context, _responseHelper, _apiUrls, HttpContext);
                    if (helper1.IsInitialized)
                    {
                        string ClientIDs = HttpContext.Session.GetString("PortalUserClientsAdditionalIds") ?? "0";
                        if (!string.IsNullOrEmpty(ClientIDs))
                        {
                            var RS1 = await helper.GetClientByIds(root.clientID_ACTIVE_SESSION);
                            if (RS1 == null)
                            {
                                return View("NoData");
                            }

                            var firstRecord = RS1.FirstOrDefault();

                            if (firstRecord.clientPhotoPath1 != null)
                            {
                                string encoded = Uri.EscapeDataString(firstRecord.clientPhotoPath1);
                                ViewBag.ClientPhotoPath1 = encoded;
                            }
                        }
                    }
                }
                // If there is no active session, you might return the "NoData" view here
                else
                {
                    // The next line is commented out, but you would typically return the "NoData" view here
                    // return View("NoData");
                }
            }
            
            // Set debug information in the ViewBag for the view to use if needed
            ViewBag.DebugMessage = debugMessage;
            ViewBag.JsonString = jsonString;

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
                catch (JsonException jsonEx)
                {
                    // Log this specific exception before proceeding
                    Console.WriteLine(jsonEx);
                    return View("Error");
                }

                var authKey = myObject1.AuthKey ?? string.Empty;

                if (string.IsNullOrEmpty(authKey))
                {
                    TempData["ErrorMessage"] = "Failed to Authenticate";
                    return View("Index");
                }

                // Get client details
                var URL1 = _apiUrls.ClientDetails + "?clientID=" + clientId.ToString();
                var (root1, debugMessage1, jsonString1) = await _responseHelper.GetResponse<Root>(authKey, URL1);

                if (root1 == null)
                {
                    return View("NoData");
                }

                // Initialize helper
                var helper = new ClientDataHelper(_context, _responseHelper, _apiUrls, HttpContext);

                if (!helper.IsInitialized)
                    return View("NoData");

                var clients = await helper.GetClientByIds(clientId.ToString());

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

                var matchingClients = clients.Where(c => c.clientID == clientId.Value).ToList();

                var partialModel = new BudgetandClients
                {
                    ClientDetails = clients,
                    Budget = RootBudget
                };

                ViewBag.Msg1 = "BudgetTest";
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
