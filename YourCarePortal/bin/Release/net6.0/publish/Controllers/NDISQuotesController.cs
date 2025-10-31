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
            UrlParameterService urlParameterService)

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
        }

        public async Task<IActionResult> NDISQuotes()

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
            logHelper.LogPageAccess("NDISQuotes");



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
            string authKey;
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

            // Authenticate the user with the provided auth key
            var Authentication1 = await _authHelper.Authenticate(authKey1);

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
                    ViewBag.NoDataMessage = _noDataMsg.NDISQuotesMainText;
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

            // Create a HttpClient instance to make HTTP requests
            var httpClient = _clientFactory.CreateClient();

            // Initialize the helper with the necessary dependencies
            var helper = new AllAPIHelperNDISQuotes(httpClient, _apiUrls.NDISQuotes);

            // Variable to hold the password reset sequence response
            var RootNDISQuotes = (RootNDISQuotes)null;

            string clientIDs = HttpContext.Session.GetString("PortalUserClientsAdditionalIds") ?? "0";

            //
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

            // Initialize the client data helper
            var helper1 = new ClientDataHelper(_context, _responseHelper, _apiUrls, HttpContext);
            if (!helper1.IsInitialized)
            {
                return View("NoData");
            }

            //Client ID is from the NDIS Quotes Packet.

            // [**] Fetch the client data; if none is found, return the NoData view
            // to be updated later as fixed for now.
            var clients = await helper1.GetClientByIds(clientIDs);
            if (clients == null || !clients.Any())
            {
                ViewBag.NoDataMessage = _noDataMsg.NDISQuotesMainText;
                ViewBag.NoDataSubText = _noDataMsg.NDISQuotesSubText;
                return View("NoData");
            }

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

                //// Create a new HttpClient instance using the factory to send requests
                var httpClient = _clientFactory.CreateClient();

                // Initialize the helper to process all API responses
                AllAPIResponseHelper allAPIResponseHelper = new AllAPIResponseHelper(httpClient, _apiUrls.NDISQuotesSignature);

                var helper = allAPIResponseHelper;

                //string cc = HttpContext.Session.GetString("portalUserID") ?? "0";
                //var dd = HttpContext.Session.GetString("zzz") ?? "0";

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

                // T & Cs
                // Make changes to display on View.
                // change supporting classes / Root classes
                // change view to display new data.



                // Initialize helper for NDIS quotes (if applicable)
                // [Add any initialization logic for NDIS quotes]

                // Process and handle the NDIS quotes data
                // [Add logic to process the NDIS quotes data]

                // Prepare the view model
                //var viewModel = new NDISQuotesViewModel
                //{
                //    // Populate the view model with NDIS quotes data
                //};

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

                // Return the view with the prepared model
                //return View("NDISQuotesPartial");

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
