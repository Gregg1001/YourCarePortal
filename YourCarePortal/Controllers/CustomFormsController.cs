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
    public class CustomFormsController : Controller
    {
        private readonly ILogger<CustomFormsController> _logger;
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
        private readonly ClientService _clientService;
        private readonly SessionPreLoadDataService _sessionPreLoadDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomFormsController(
            ILogger<CustomFormsController> logger,
            AuthenticationHelper authHelper,
            ChangePasswordHelper changePasswordHelper,
            ResponseHelper responseHelper,
            IOptions<ApiUrls> apiUrls,
            IOptions<NoDataMsg> noDataMsg,
            DatabaseContext context,
            IMemoryCache memoryCache,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            ClientService clientService,
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
            _clientService = clientService;
            _sessionPreLoadDataService = sessionPreLoadDataService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Method to handle the CustomForms action for a controller in an ASP.NET Core MVC application.
        /// It fetches and displays a list of custom forms associated with an authenticated user.
        /// </summary>
        /// <returns>ActionResult representing the result of the CustomForms action</returns>
        /// 
        public async Task<IActionResult> CustomForms()
        {
            var authKey = string.Empty;
            var portalUserEmail = string.Empty;
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

            // Retrieve Auth Key from Cookie
            // Attempt to fetch  data using the Budget Pre Load Service

            //var (WhiteLabelLocation, Authentication, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

            //var authKey = myObject1.AuthKey ?? string.Empty;

            //// Check if the user is authenticated
            //if (string.IsNullOrEmpty(authKey))
            //{
            //    Response.Cookies.Delete("TP_YCP_1");
            //    // Clear all session variables
            //    HttpContext.Session.Clear();
            //    if (!string.IsNullOrEmpty(WhiteLabelLocation))
            //    {
            //        ViewBag.WhiteLabel = WhiteLabelLocation;
            //    }
            //    return View("Index");
            //}

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


            // Check and set debug flag from the Query string
            if (Request.Query.ContainsKey("Debug"))
            {
                string debugValue = Request.Query["Debug"].ToString().ToLower();
                HttpContext.Session.SetString("YCP_Debug", debugValue == "true" ? "true" : "false");
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

            if (Authentication != null)
            {
                ViewBag.SETTING_StatementsEnabled = Authentication.SETTING_StatementsEnabled;
                ViewBag.SETTING_BudgetEnabled = Authentication.SETTING_BudgetEnabled;
                ViewBag.SETTING_SupportplanEnabled = Authentication.SETTING_SupportplanEnabled;
                ViewBag.SETTING_FormsEnabled = Authentication.SETTING_FormsEnabled;
                ViewBag.SETTING_NDIS_StatementsEnabled = Authentication.SETTING_NDIS_StatementsEnabled;
                ViewBag.SETTING_NDIS_Quotes_Enabled = Authentication.SETTING_NDIS_Quotes_Enabled;

                if (Authentication.SETTING_BudgetEnabled == "no")
                {
                    ViewBag.NoDataMessage = _noDataMsg.CustomFormsMainText;
                    ViewBag.NoDataSubText = _noDataMsg.CustomFormsSubText;
                    return View("NoData");
                }
            }

            if (WhiteLabelLocation != null)
            {
                ViewBag.WhiteLabel = WhiteLabelLocation;
            }

            // Retrieve client IDs fro
            string clientIDs = Authentication?.LinkedClientIDS ?? "null";

            // If no client IDs are found, show a relevant message
            if (string.IsNullOrEmpty(clientIDs))
            {
                ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                return View("NoData");
            }

            // Initialize the client data helper
            var helper = new ClientDataHelper(_context, _responseHelper, _apiUrls, HttpContext);
            if (!helper.IsInitialized)
            {
                ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                return View("NoData");
            }

            // Fetch clients based on client IDs
            //var clients = await helper.GetClientByIds(clientIDs);
            if (clients == null || !clients.Any())
            {
                ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                return View("NoData");
            }

            // Example usage of ClientService methods
            //string clientIDs = "123,456,789";
            int firstClientId = await _clientService.GetFirstClientId(clientIDs);
            var URL3 = _apiUrls.FormsList + "?cid=" + firstClientId.ToString();

            var (root, debugMessage, jsonString) = await _responseHelper.GetResponse<RootFormsListPerson>(authKey, URL3);

            // Check if data retrieval was successful
            if (root == null)
            {
                return View("NoData");
            }

            //Log Page Access
            //ILogAccess logger = new UpdatePortalAccessLog();
            //LogAccessHelper logHelper = new LogAccessHelper(logger, HttpContext);
            //logHelper.LogPageAccess("CustomForms");

            // Pass any debug messages and JSON string to the view
            ViewBag.DebugMessage = debugMessage;
            ViewBag.JsonString = jsonString;

            // Check for the 'Debug' mode based on URL query parameters
            if (Request.Query.ContainsKey("Debug"))
            {
                string debugValue = Request.Query["Debug"].ToString().ToLower();
                HttpContext.Session.SetString("YCP_Debug", debugValue == "true" ? "true" : "false");
            }

            // Create a model object to send to the view
            var model = new FormsandClients
            {
                ClientDetails = clients,
                RootFormsListPerson = root  
            };

            return View("Formslist", model);
        }

        /// <summary>
        /// Fetches and displays details about custom forms based on the provided ID.
        /// </summary>
        /// <param name="id">The identifier of the custom form.</param>
        /// <returns>An IActionResult representing the outcome of the fetch operation.</returns>
        ///
        [HttpGet]
        public async Task<IActionResult> FormsListAction(string id)
        {
            try
            {
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                Cookie myObject1;

                try
                {
                    myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deserializing the JSON string: " + ex.Message);
                    return Json(new { success = false, message = "Invalid cookie data." });
                }

                var authKey = myObject1.AuthKey ?? string.Empty;

                // If the auth key is missing or empty, redirect to the Index view with an error message
                if (string.IsNullOrEmpty(authKey))
                {
                    TempData["ErrorMessage"] = "Failed to Authenticate";
                    return View("Index");
                }

                // Construct the API URL using the provided form ID
                var URL1 = _apiUrls.CustomForms + "?customFormEntryId=" + id;

                // Make a request to the API to fetch details about the custom form
                var (root, debugMessage, jsonString) = await _responseHelper.GetResponse<RootCustomForms>(authKey, URL1);

                // If no response is received from the API, return the NoData view
                if (root == null)
                {
                    return View("NoData");
                }

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
                }

                //White Label
                string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";

                // Fetch the image location using the service
                var WhiteLabelLocation = _imageLocationService.GetImageLocationFromUrl(currentUrl);
                if (WhiteLabelLocation != null)
                    ViewBag.WhiteLabel = WhiteLabelLocation;
                
                // Display the details of the fetched custom form
                return View("CustomForms", root);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging and return an error response
                Console.WriteLine(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Handles form submission for a signature field.
        /// </summary>
        /// <param name="signatureCode">The code representing the signature.</param>
        /// <param name="signatureFieldID">The identifier of the signature field in the form.</param>
        /// <param name="customFormId">The identifier of the custom form being submitted.</param>
        /// <returns>A JSON result indicating success or failure, or a redirect to a view upon completion.</returns>
        /// 
        [HttpPost]
        public async Task<IActionResult> FormsSubmitAction(string signatureCode, string signatureFieldID, string customFormId)
        {
            // Retrieve the authentication key from cookies
            string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
            Cookie myObject1;

            // Try deserializing the JSON string and return an error JSON response if it fails
            try
            {
                myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
            }
            catch (Exception ex)
            {
                // Log the exception message to the console or any other logging service
                Console.WriteLine("Error deserializing the JSON string: " + ex.Message);
                // Return a JSON response indicating the failure
                return Json(new { success = false, message = "Invalid cookie data." });
            }

            // Extract the authentication key from the deserialized cookie object
            var authKey = myObject1?.AuthKey ?? string.Empty;

            // Create a new HttpClient instance using the factory to send requests
            var httpClient = _clientFactory.CreateClient();

            // Initialize the helper to process all API responses
            var helper = new AllAPIResponseHelper(httpClient, _apiUrls.FormsSignature);

            // Send the request to the server with the signature data and other form details
            var ChangePassword = await helper.SendRequest<RootCustomForms>(
                new KeyValuePair<string, string>("authKey", authKey),
                new KeyValuePair<string, string>("customFormEntryId", customFormId),
                new KeyValuePair<string, string>("customFormEntryFieldID", signatureFieldID),
                new KeyValuePair<string, string>("signature", signatureCode)
            );

            // Include error handling logic here if necessary
            // For example, if the response from the server indicates an error, handle it accordingly

            // Redirect to a view or return a JSON response upon completion of the form submission process
            // Depending on the business logic, you might want to check if the ChangePassword result is successful
            return View();
        }

        /// <summary>
        /// Asynchronous action method to fetch client statements based on an optional client ID.
        /// </summary>
        /// <param name="clientId">Optional client ID to filter the forms.</param>
        /// <returns>IActionResult containing either statements for a client or an error message.</returns>
        ///
        [HttpGet]
        public async Task<IActionResult> FormsAction(int? clientId)
        {
            try
            {
                // Retrieve cookie containing authentication data
                string jsonString = Request.Cookies["TP_YCP_1"] ?? string.Empty;

                // Deserialize the cookie data to get the authentication key
                Cookie myObject1;
                try
                {
                    myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString);
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

                // Fetch client details from API
                var URL1 = _apiUrls.ClientDetails + "?clientID=" + clientId.ToString();
                var (root1, debugMessage1, jsonString1) = await _responseHelper.GetResponse<Root>(authKey, URL1);

                // Verify if API response is valid
                if (root1 == null)
                {
                    return View("NoData");
                }

                // Attempt to fetch data using the Pre Load Service
                var (_, _, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

                // Verify if clients are present
                if (clients == null || !clients.Any())
                {
                    ViewBag.NoDataMessage = _noDataMsg.MainText;
                    ViewBag.NoDataSubText = _noDataMsg.SubText;
                    return View("NoData");
                }

                var URL2 = _apiUrls.FormsList + "?cid=" + clientId.ToString();
                // Fetch client statements
                var (root, debugMessage, jsonString2) = await _responseHelper.GetResponse<RootFormsListPerson>(authKey, URL2);

                // Verify if statements are present
                if (root == null)
                {
                    ViewBag.NoDataMessage = _noDataMsg.CustomFormsMainText;
                    ViewBag.NoDataSubText = _noDataMsg.CustomFormsSubText;
                    return View("NoData");
                }

                ViewBag.DebugMessage = debugMessage;
                ViewBag.JsonString = jsonString2;

                // Filter clients that match the provided client ID
                var matchingClients = clients.Where(c => c.clientID == clientId.Value).ToList();

                // Prepare the model for the partial view
                var partialModel = new FormsandClients
                {
                    ClientDetails = matchingClients,
                    RootFormsListPerson = root
                };

                return PartialView("FormsPartial", partialModel);
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
