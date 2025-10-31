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
    public class SettingsController : Controller
    {
        private readonly ILogger<SettingsController> _logger;
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

        public SettingsController(
            ILogger<SettingsController> logger,
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
        /// Fetches and displays user settings based on the authenticated user.
        /// </summary>
        /// <returns>An IActionResult that represents the outcome of the settings retrieval operation.</returns>
        public async Task<IActionResult> Settings()
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
            logHelper.LogPageAccess("Settings");


            // Retrieve the authentication key from the cookie
            var jsonString3 = Request.Cookies["TP_YCP_1"]?.ToString();

            // Safely attempt to deserialize the JSON string into a Cookie object
            Cookie myObject1;
            try
            {
                myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
            }
            catch (JsonException)
            {
                TempData["ErrorMessage"] = "Failed to parse authentication data.";
                return View("Index");
            }

            var authKey = myObject1?.AuthKey;

            // Validate the authentication key
            if (string.IsNullOrEmpty(authKey))
            {
                TempData["ErrorMessage"] = "Failed to Authenticate";
                return View("Index");
            }

            // Authenticate the user and retrieve their settings
            var Authentication1 = await _authHelper.Authenticate(authKey);
            if (Authentication1 != null)
            {
                // Set the ViewBag properties based on user settings
                ViewBag.SETTING_StatementsEnabled = Authentication1.SETTING_StatementsEnabled;
                ViewBag.SETTING_BudgetEnabled = Authentication1.SETTING_BudgetEnabled;
                ViewBag.SETTING_SupportplanEnabled = Authentication1.SETTING_SupportplanEnabled;

                // Handle cases where Statements are not enabled
                if (Authentication1.SETTING_StatementsEnabled == "no")
                {
                    ViewBag.NoDataMessage = _noDataMsg.SettingsMainText;
                    ViewBag.NoDataSubText = _noDataMsg.SettingsSubText;
                    return View("NoData");
                }
            }

            // Check for the 'Debug' mode based on URL query parameters
            if (Request.Query.ContainsKey("Debug"))
            {
                var debugValue = Request.Query["Debug"].ToString().ToLower();
                HttpContext.Session.SetString("YCP_Debug", debugValue == "true" ? "true" : "false");
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

            // Fetch the Notification Settings from the API
            var (root, debugMessage, jsonString) = await _responseHelper.GetResponse<RootSettingsNotification>(authKey, _apiUrls.ChangeNotification);

            ViewBag.DebugMessage = debugMessage;
            ViewBag.JsonString = jsonString;

            // Handle cases where the response is missing or empty
            if (string.IsNullOrEmpty(jsonString))
            {
                return View("NoData");
            }

            // Store the notification settings in the ViewBag for rendering in the view
            ViewBag.NotificationSettings = root.Notifications ?? "";

            ViewBag.Title = "Settings - Your Care Portal";

            return View();
        }

        /// <summary>
        /// Handles password change requests.
        /// </summary>
        /// <param name="password_change1">The current password.</param>
        /// <param name="password_change2">The new password.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> SettingsAction(string? password_change1, string? password_change2)
        {
            try
            {
                // Read and deserialize the user authentication cookie
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                var myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
                var authKey = myObject1.AuthKey ?? string.Empty;

                // If the auth key is missing or empty, redirect to the Index view with an error message
                if (string.IsNullOrEmpty(authKey))
                {
                    TempData["ErrorMessage"] = "Failed to Authenticate";
                    return View("Index");
                }

                var httpClient = _clientFactory.CreateClient();

                // Initialize API helper to manage password change request
                var helper = new APIResponseHelper(httpClient, _apiUrls.ChangePassword);

                // Request to change the password using the provided credentials and auth key
                var ChangePassword = await helper.ChangePassword(
                    new KeyValuePair<string, string>("authKey", authKey),
                    new KeyValuePair<string, string>("password_change1", password_change1 ?? string.Empty),
                    new KeyValuePair<string, string>("password_change2", password_change2 ?? string.Empty)
                );

                // Process the response from the password change request
                if (ChangePassword.message == null)
                {
                    // No data available to display
                    ViewBag.NoDataMessage = _noDataMsg.SettingsMainText;
                    ViewBag.NoDataSubText = _noDataMsg.SettingsSubText;
                    return View("NoData");
                }
                else if (ChangePassword.message == "Password Updated Successfully. Please log out before proceeding.")
                {
                    // Password updated successfully
                    ViewBag.ShowPasswordSuccess = "true";

                    // Update the authentication cookie with the new auth key
                    var jsonString1 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                    var cookieObject = JsonConvert.DeserializeObject<dynamic>(jsonString1);
                    CookieOptions options2 = new CookieOptions { Expires = DateTime.Now.AddDays(2) };
                    cookieObject.AuthKey = ChangePassword.newAUTHKEY;
                    string updatedCookieValue = JsonConvert.SerializeObject(cookieObject);
                    Response.Cookies.Append("TP_YCP_1", updatedCookieValue, options2);
                }
                else
                {
                    // Display the error message from the password change response
                    ViewBag.passwordErrorMessage = ChangePassword.message;
                }

                return PartialView("SettingsPartial");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging and return an error response
                Console.WriteLine(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }



        /// <summary>
        /// Handles user notification settings.
        /// </summary>
        /// <param name="Setting1">The notification setting value (e.g., Notify Daily, Notify Weekly, etc.).</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpGet]
        public async Task<IActionResult> SettingsNotificationAction(string Setting1)
        {
            // If the provided setting is null, reset the selection and return the partial view
            if (Setting1 == null)
            {
                ViewBag.NotificationSettings = "none";
                return PartialView("SettingsPartial");
            }

            try
            {
                // Read and deserialize the user authentication cookie
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                var myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
                var authKey = myObject1.AuthKey ?? string.Empty;

                // If the auth key is missing or empty, redirect to the Index view with an error message
                if (string.IsNullOrEmpty(authKey))
                {
                    TempData["ErrorMessage"] = "Failed to Authenticate";
                    return View("Index");
                }

                // Create a HttpClient instance using IHttpClientFactory
                var httpClient = _clientFactory.CreateClient();

                // Construct the API URL with the provided notification setting value
                var URL1 = _apiUrls.ChangeNotification + "?portalUserNotificationSettings=" + Setting1.Replace(" ", "%20");

                // Make a request to the API to update the notification settings
                var (root1, debugMessage1, jsonString1) = await _responseHelper.GetResponse<RootSettingsNotification>(authKey, URL1);

                // If no response is received from the API, return the NoData view
                if (root1 == null)
                {
                    return View("NoData");
                }

                // Check if the notification setting was updated successfully
                if (root1.Notifications != Setting1)
                {
                    ViewBag.ShowNotificationSuccess = "false";
                    ViewBag.notificationErrorMessage = "Failed to update Notification";
                }
                else if (root1.Notifications == Setting1)
                {
                    ViewBag.ShowNotificationSuccess = "true";
                }

                return PartialView("SettingsPartial");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging and return an error response
                Console.WriteLine(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
