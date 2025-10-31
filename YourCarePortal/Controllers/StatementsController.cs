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
    public class StatementsController : Controller
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

        public StatementsController(
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
        /// Fetches and displays statement details based on authenticated user.
        /// </summary>
        /// <returns>An IActionResult that represents the outcome of the statement retrieval operation.</returns>
        public async Task<IActionResult> Statements()
        {

            // Attempt to retrieve the authentication key from the cookie, if not found, use an empty string
            //string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
            //var myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);

            ////here
            //var authKey = myObject1.AuthKey ?? string.Empty;
            //var portalUserEmail = myObject1?.portalUserEmail ?? string.Empty;
            // Attempt to fetch  data using the Budget Pre Load Service

            var authKey = string.Empty;
            var portalUserEmail = string.Empty;
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

            //// Attempt to fetch  data using the Budget Pre Load Service
            //var (WhiteLabelLocation, Authentication, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

            //### If Portal Access is not authorised (can be due to password changed in TP!
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

            // Check for the 'Debug' mode based on URL query parameters
            if (Request.Query.ContainsKey("Debug"))
            {
                string debugValue = Request.Query["Debug"].ToString().ToLower();
                HttpContext.Session.SetString("YCP_Debug", debugValue == "true" ? "true" : "false");
            }

            // Retrieve client IDs from the session
            string clientIDs = HttpContext.Session.GetString("PortalUserClientsAdditionalIds") ?? "0";

            // If no client IDs are found, show a relevant message
            if (string.IsNullOrEmpty(clientIDs))
            {
                ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                return View("NoData");
            }

            // InitializeSession Data
            if (HttpContext.Session.GetString("portalUserID") == null)
            {
                var sessionManager = new SessionManager(_context, _responseHelper, _apiUrls, HttpContext, portalUserEmail);
                sessionManager.InitializeSession();
            }

            // Get client details and switch clients
            string client1 = clients.FirstOrDefault().clientID.ToString();
            var URL1 = _apiUrls.ClientDetails + "?clientID=" + client1;
            var (root1, debugMessage1, jsonString1) = await _responseHelper.GetResponse<Root>(authKey, URL1);

            // Fetch statement details from the API
            var (RootStatements, debugMessage, jsonString) = await _responseHelper.GetResponse<RootStatements>(authKey, _apiUrls.Statements);

            // Handle cases where statement details are missing
            if (RootStatements == null)
            {
                ViewBag.NoDataMessage = _noDataMsg.StatementsMainText;
                ViewBag.NoDataSubText = _noDataMsg.StatementsSubText;
            }

            // Store debugging details in the ViewBag for rendering in the view
            ViewBag.DebugMessage = debugMessage;
            ViewBag.JsonString = jsonString;

            // Create a model object to send to the view
            var model = new StatementsandClients
            {
                ClientDetails = clients,
                Statements = RootStatements
            };

            // Assuming you have the DateTime value for the cut-off date
            //DateTime cutoffDate = new DateTime(2024, 9, 1);
            var cutoffDate = _configuration.GetValue<DateTime>("StatementSettings:CutoffDate");

            ViewBag.CutOffsetDate = cutoffDate.ToString("dd/MM/yyyy");

            if (RootStatements != null)
            {
                // Filter the StatementsList based on the cut-off date
                var StatementsList1 = model.Statements.StatementsList
                    .Where(statement => DateTime.ParseExact(statement.Month, "MMM yyyy", CultureInfo.InvariantCulture) >= cutoffDate)
                    .ToList();

                // Update the StatementsList in the model
                if (StatementsList1 != null)
                {
                    model.Statements.StatementsList = StatementsList1;
                }
            }

            // Log Page Access
            ILogAccess logger = new UpdatePortalAccessLog();
            LogAccessHelper logHelper = new LogAccessHelper(logger, HttpContext);
            logHelper.LogPageAccess("Statements");

            ViewBag.Title = "Statements - Your Care Portal";
            return View("Statements", model);
        }

        /// <summary>
        /// Asynchronous action method to fetch client statements based on an optional client ID.
        /// </summary>
        /// <param name="clientId">Optional client ID to filter the statements.</param>
        /// <returns>IActionResult containing either statements for a client or an error message.</returns>
        
        [HttpGet]
        public async Task<IActionResult> StatementsAction(int? clientId)
        {
            try
            {
                // Retrieve cookie containing authentication data
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;

                // Deserialize the cookie data to get the authentication key
                Cookie myObject1;
                try
                {
                    myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
                }
                catch (Exception ex)
                {
                    // Handle JSON deserialization error
                    Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                    return View("Error", new { message = "Invalid Cookie Data" });
                }

                // Extract the authentication key
                var authKey = myObject1?.AuthKey ?? string.Empty;

                // Check authentication key validity
                if (string.IsNullOrEmpty(authKey))
                {
                    TempData["ErrorMessage"] = "Failed to Authenticate";
                    return View("Index");
                }

                // Attempt to fetch  data using the Pre Load Service
                var (_, _, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

                // Verify if clients are present
                if (clients == null || !clients.Any())
                {
                    ViewBag.NoDataMessage = _noDataMsg.MainText;
                    ViewBag.NoDataSubText = _noDataMsg.SubText;
                    return View("NoData");
                }

                // Get client details and switch clients
                var URL1 = _apiUrls.ClientDetails + "?clientID=" + clientId.ToString();
                var (root1, debugMessage1, jsonString1) = await _responseHelper.GetResponse<Root>(authKey, URL1);

                // Fetch client statements
                var (RootStatements, debugMessage, jsonString) = await _responseHelper.GetResponse<RootStatements>(authKey, _apiUrls.Statements);

                // Verify if statements are present
                if (RootStatements == null || RootStatements.clientID_ACTIVE_SESSION == null)
                {
                    ViewBag.NoDataMessage = _noDataMsg.StatementsMainText;
                    ViewBag.NoDataSubText = _noDataMsg.StatementsSubText;
                }

                ViewBag.DebugMessage = debugMessage;
                ViewBag.JsonString = jsonString;

                // Filter clients that match the provided client ID
                var matchingClients = clients.Where(c => c.clientID == clientId.Value).ToList();

                // Prepare the model for the partial view
                var partialModel = new StatementsandClients
                {
                    ClientDetails = matchingClients,
                    Statements = RootStatements // Further filter based on client ID if necessary
                };

                return PartialView("StatementsPartial", partialModel);
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Console.WriteLine(ex);

                // Return a JSON object with error information
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Fetches and displays statement details for a given client ID and month.
        /// </summary>
        /// <param name="month">The month for which statement details are to be fetched.</param>
        /// <param name="clientID">The ID of the client whose statements are to be retrieved.</param>
        /// <returns>An IActionResult that represents the outcome of the statement details retrieval operation.</returns>
        public async Task<IActionResult> StatementsDetails(string month, int clientID)
        {
            var httpContext = _httpContextAccessor.HttpContext;
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

            var authKey = myObject1.AuthKey ?? string.Empty;

            // Validate the authentication key
            if (string.IsNullOrEmpty(authKey))
            {
                TempData["ErrorMessage"] = "Failed to Authenticate";
                return View("Index");
            }

            // Detect and handle the 'Debug' mode based on URL query parameters
            if (Request.Query.ContainsKey("Debug"))
            {
                string debugValue = Request.Query["Debug"].ToString().ToLower();
                HttpContext.Session.SetString("YCP_Debug", debugValue == "true" ? "true" : "false");
            }

            // Attempt to fetch  data using the Budget Pre Load Service
            var (WhiteLabelLocation, Authentication, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

            // Construct the API URL to fetch statement details
            var URL1 = _apiUrls.StatementDetails + "?month=" + month.Replace(" ", "");
            var (RootStatementDetails, debugMessage, jsonString) = await _responseHelper.GetResponse<RootStatementDetails>(authKey, URL1);

            // Check the validity of fetched statement details
            if (RootStatementDetails == null)
            {
                TempData["ErrorMessage"] = debugMessage ?? "Failed to get the data, try logging in again.";
                return View("Index");
            }

            // Store debugging details in the ViewBag for rendering in the view
            ViewBag.DebugMessage = debugMessage;
            ViewBag.JsonString = jsonString;
            ViewBag.StatmentMonth = month;

            // Authenticate the user again and retrieve their settings (consider caching this to avoid frequent calls)
            var Authentication1 = await _authHelper.Authenticate(authKey);
            if (Authentication1 != null)
            {
                ViewBag.SETTING_StatementsEnabled = Authentication1.SETTING_StatementsEnabled;
                ViewBag.SETTING_BudgetEnabled = Authentication1.SETTING_BudgetEnabled;
                ViewBag.SETTING_SupportplanEnabled = Authentication1.SETTING_SupportplanEnabled;
                ViewBag.SETTING_FormsEnabled = Authentication1.SETTING_FormsEnabled;
                ViewBag.SETTING_NDIS_StatementsEnabled = Authentication1.SETTING_NDIS_StatementsEnabled;
                ViewBag.SETTING_NDIS_Quotes_Enabled = Authentication1.SETTING_NDIS_Quotes_Enabled;

                if (Authentication1.SETTING_BudgetEnabled == "no")
                {
                    ViewBag.NoDataMessage = _noDataMsg.BudgetMainText;
                    ViewBag.NoDataSubText = _noDataMsg.BudgetSubText;
                    return View("NoData");
                }
            }

            if (clients == null || !clients.Any())
            {
                ViewBag.NoDataMessage = _noDataMsg.MainText;
                ViewBag.NoDataSubText = _noDataMsg.SubText;
                return View("NoData");
            }

            // Filter the client list based on the provided client ID
            var matchingClients = clients.Where(c => c.clientID == clientID).ToList();

            // Create a model object to send to the view
            var partialModel = new StatementDetailsandClients
            {
                ClientDetails = matchingClients,
                StatementDetails = RootStatementDetails
            };

            //Log Page Access
            ILogAccess logger = new UpdatePortalAccessLog();
            LogAccessHelper logHelper = new LogAccessHelper(logger, HttpContext);
            logHelper.LogPageAccess("StatementsDetails");

            if (WhiteLabelLocation != null)
                ViewBag.WhiteLabel = WhiteLabelLocation;

            return View("StatementsDetails", partialModel);
        }
    }
}
