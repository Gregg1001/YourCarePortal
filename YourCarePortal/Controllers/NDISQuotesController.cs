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
    public class NDISQuotesController : Controller
    {
        private readonly ILogger<NDISQuotesController> _logger;
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
        private readonly UrlParameterService _urlParameterService;
        private readonly SessionPreLoadDataService _sessionPreLoadDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NDISQuotesController(
            ILogger<NDISQuotesController> logger,
            AuthenticationHelper authHelper,
            ChangePasswordHelper changePasswordHelper,
            ResponseHelper responseHelper,
            IOptions<ApiUrls> apiUrls,
            IOptions<NoDataMsg> noDataMsg,
            DatabaseContext context,
            IMemoryCache memoryCache,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            UrlParameterService urlParameterService,
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
            _urlParameterService = urlParameterService;
            _sessionPreLoadDataService = sessionPreLoadDataService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> NDISQuotes()
        {
            // Obtain the current URL
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";

            var (WhiteLabelLocation, Authentication, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

            // If the cookie is missing, redirect to the Index view
            if (!Request.Cookies.ContainsKey("TP_YCP_1"))
            {
                if (!string.IsNullOrEmpty(WhiteLabelLocation)){
                    ViewBag.WhiteLabel = WhiteLabelLocation;
                }
                TempData["ErrorMessage"] = "Email or Password incorrect. Please try again.";
                return View("Index");
            }

            if (!Request.Cookies.ContainsKey("TP_YCP_1"))
            {

                // Extract parameter from URL using the service
                int NDISQuoteNo1 = _urlParameterService.GetParameterFromUrl(currentUrl);

                // Set for redirection
                ViewBag.ViewName = "NDISQuotes/"+ NDISQuoteNo1;

                return View("Index");
            }

            // Set the controller name in the session
            HttpContext.Session.SetString("ControllerName", "NDISQuotes" ?? "null");

            // Variable to store the authentication key
            var myObject1 = new Cookie();

            try
            {
                // Attempt to retrieve the authentication key from the cookie, if not found, use an empty string
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
            }
            catch (Exception ex)
            {
                // Log the error
                TempData["ErrorMessage"] = "An error occurred.";
                return View("Error");  // Assuming you have an error view
            }

            // Safely extract the AuthKey from the deserialized cookie object
            var authKey1 = myObject1.AuthKey ?? string.Empty;

            // If the authentication key is missing, set an error message and redirect to the Index view
            if (string.IsNullOrEmpty(authKey1))
            {
                TempData["ErrorMessage"] = "Failed to Authenticate";
                return View("Index");
            }

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

            // If authentication is successful, store settings in the ViewBag for use in the view
            if (Authentication != null)
            {
                ViewBag.SETTING_StatementsEnabled = Authentication.SETTING_StatementsEnabled;
                ViewBag.SETTING_BudgetEnabled = Authentication.SETTING_BudgetEnabled;
                ViewBag.SETTING_SupportplanEnabled = Authentication.SETTING_SupportplanEnabled;
                ViewBag.SETTING_FormsEnabled = Authentication.SETTING_FormsEnabled;
                ViewBag.SETTING_NDIS_StatementsEnabled = Authentication.SETTING_NDIS_StatementsEnabled;
                ViewBag.SETTING_NDIS_Quotes_Enabled = Authentication.SETTING_NDIS_Quotes_Enabled;

                // Store additional client IDs in the session
                HttpContext.Session.SetString("PortalUserClientsAdditionalIds", Authentication?.LinkedClientIDS ?? "null");
            }

            if (WhiteLabelLocation != null)
            {
                ViewBag.WhiteLabel = WhiteLabelLocation;
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

            // Create a HttpClient instance to make HTTP requests
            var httpClient = _clientFactory.CreateClient();

            // Initialize the helper with the necessary dependencies
            var helper = new AllAPIHelperNDISQuotes(httpClient, _apiUrls.NDISQuotes);

            // Variable to hold the password reset sequence response
            var RootNDISQuotes = (RootNDISQuotes)null;

            string clientIDs = HttpContext.Session.GetString("PortalUserClientsAdditionalIds") ?? "0";

            var helperP = new ProviderDataHelper(_context, _responseHelper, _apiUrls, HttpContext);

            if (!helperP.IsInitialized)
            {
                ViewBag.NoDataMessage = _noDataMsg.NDISQuotesMainText;
                ViewBag.NoDataSubText = _noDataMsg.NDISQuotesSubText;
                return View("NoData"); // Again, consider a different message indicating helper initialization issues
            }
            var providers = await helperP.GetProvidersByClientIds(clientIDs);
            if (providers == null || !providers.Any())
            {
                ViewBag.NoDataMessage = _noDataMsg.NDISQuotesMainText;
                ViewBag.NoDataSubText = _noDataMsg.NDISQuotesSubText;
                return View("NoData");
            }

            string secret = _configuration.GetValue<String>("SecretSetting:Secret");

            // Extract parameter from URL using the service
            int NDISQuoteNo = _urlParameterService.GetParameterFromUrl(currentUrl);

            RootNDISQuotes = await helper.SendRequest<RootNDISQuotes>(
                new KeyValuePair<string, string>("quote", NDISQuoteNo.ToString()),
                new KeyValuePair<string, string>("secret", secret),

                new KeyValuePair<string, string>("provider", providers), // "1,2"),
                new KeyValuePair<string, string>("client", clientIDs) // "151,152")
            );

            if (RootNDISQuotes.Status != "OK")
            {
                return View("NoData");
            }

            if (clients == null || !clients.Any())
            {
                ViewBag.NoDataMessage = _noDataMsg.NDISQuotesMainText;
                ViewBag.NoDataSubText = _noDataMsg.NDISQuotesSubText;
                return View("NoData");
            }

            //Log Page Access
            ILogAccess logger = new UpdatePortalAccessLog();
            LogAccessHelper logHelper = new LogAccessHelper(logger, HttpContext);
            logHelper.LogPageAccess("NDISQuotes");

            // Filter the clients to match the NDIS Quotes Packet client ID
            var matchingClients = clients.Where(c => c.clientID.ToString() == RootNDISQuotes.QhClientNo).ToList();

            //Create a new instance of the model with clients and NDISQuotes
            var NDISQuotesandClients = new NDISQuotesandClients
            {
                ClientDetails = matchingClients,
                NDISQuotes = RootNDISQuotes
            };

            // Set the title for the view and return the Budget view with the model
            ViewBag.Title = "NDIS Quotes";
            //return View("NDISQuotes", RootNDISQuotes);

            return View("NDISQuotes", NDISQuotesandClients);
        }

        public async Task<IActionResult> NDISQuotesAction(int? QhCompanyNo, int? QhClientNo, int? QhClientNDISNumber, string Qh_ClientSigner, string QhSignature, string Id)
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

                //// Create a new HttpClient instance using the factory to send requests
                var httpClient = _clientFactory.CreateClient();

                // Initialize the helper to process all API responses
                AllAPIResponseHelper allAPIResponseHelper = new AllAPIResponseHelper(httpClient, _apiUrls.NDISQuotesSignature);

                var helper = allAPIResponseHelper;

                string portalUserFullname = HttpContext.Session.GetString("portalUserFullname") ?? "0";

                string secret = _configuration.GetValue<String>("SecretSetting:Secret");
                string provider = QhCompanyNo?.ToString() ?? string.Empty;
                string client = QhClientNo?.ToString() ?? string.Empty;
                // string signer = "portalUserFullname1"; // change to Portal User name. 
                string signature = QhSignature ?? string.Empty;
                string quote = Id?.ToString() ?? string.Empty;

                // Send the request to the server with the signature data and other form details
                var NDISQuotesSignature = await helper.SendRequest<RootNDISQuotes>(
                    new KeyValuePair<string, string>("secret", secret),
                    new KeyValuePair<string, string>("provider", provider),
                    new KeyValuePair<string, string>("client", client),
                    new KeyValuePair<string, string>("quote", quote),
                    new KeyValuePair<string, string>("signer", portalUserFullname),
                    new KeyValuePair<string, string>("signature", signature)
                );

                if (NDISQuotesSignature == null)

                {
                    ViewBag.NoDataMessage = _noDataMsg.NDISQuotesMainText;
                    ViewBag.NoDataSubText = _noDataMsg.NDISQuotesSubText;

                    return View("NoData");
                }

                // Return the view with the prepared model
                return View("NDISQuotesPartial");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> NDISQuotesPDFAction(int? userCompany, int? id)
        {
            try
            {
                // Fetch and deserialize cookie
                string jsonString = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                Cookie myObject1;
                try
                {
                    myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString);
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

                //// Create a new HttpClient instance using the factory to send requests
                var httpClient = _clientFactory.CreateClient();

                // Initialize the helper to process all API responses
                AllAPIHelperNDISQuotesPDF AllAPIHelperNDISQuotesPDF = new AllAPIHelperNDISQuotesPDF(httpClient, _apiUrls.NDISQuotesPDF);

                var helperPDF = AllAPIHelperNDISQuotesPDF;

                string portalUserFullname = HttpContext.Session.GetString("portalUserFullname") ?? "0";

                string secret = _configuration.GetValue<String>("SecretPDFSetting:Secret");

                var NDISQuotesPDF = await helperPDF.SendRequestForPdf(
                    new KeyValuePair<string, string>("secret", secret),
                    new KeyValuePair<string, string>("userCompany", userCompany.ToString()),
                    new KeyValuePair<string, string>("id", id.ToString())
                );

                if (NDISQuotesPDF == null)
                {
                    ViewBag.NoDataMessage = _noDataMsg.NDISQuotesMainText;
                    ViewBag.NoDataSubText = _noDataMsg.NDISQuotesSubText;

                    return View("NoData");
                }

                if (NDISQuotesPDF != null)
                {

                    Response.Headers.Add("Content-Disposition", "inline");
                    return File(NDISQuotesPDF, "application/pdf", "DownloadedFile.pdf");
                }
                else
                {
                    // Handle the case where the PDF is not successfully retrieved
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
