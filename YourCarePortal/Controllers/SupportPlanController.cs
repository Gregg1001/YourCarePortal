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
    public class SupportPlanController : Controller
    {
        private readonly ILogger<SupportPlanController> _logger;
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

        public SupportPlanController(
            ILogger<SupportPlanController> logger,
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
        /// Asynchronously retrieves the support plan for a user based on authentication and session information.
        /// </summary>
        /// <remarks>
        /// This method extracts user authentication from cookies, initializes session data if necessary,
        /// and retrieves the user's support plan using client IDs. It handles various conditions where
        /// data might not be available and directs to appropriate views accordingly. If the support plan
        /// feature is not enabled or on any failure, it redirects to a 'NoData' view with a corresponding message.
        /// </remarks>
        /// <returns>
        /// A Task that, when completed successfully, returns an IActionResult that represents the outcome of the operation.
        /// If successful, the 'SupportPlan' view is returned populated with model data. In cases of failure or if the feature
        /// is not enabled, it redirects to 'NoData' or 'Error' views.
        /// </returns>
        /// <exception cref="System.Exception">Thrown when an unexpected error occurs.</exception>

        public async Task<IActionResult> SupportPlan()
        {
            try
            {
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                var myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
                var authKey = myObject1?.AuthKey ?? string.Empty;
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

                if (string.IsNullOrEmpty(authKey))
                {
                    Response.Cookies.Delete("TP_YCP_1");
                    // Clear all session variables
                    HttpContext.Session.Clear();
                    if (!string.IsNullOrEmpty(WhiteLabelLocation)){
                        ViewBag.WhiteLabel = WhiteLabelLocation;}
                    return View("Index");
                }

                // Get Email from Cookie
                string jsonString31 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                var myObject11 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
                var portalUserEmail = myObject11?.portalUserEmail ?? string.Empty;
                string? email = portalUserEmail;

                //### If Portal Access is not authorised (can be due to password changed in TP!
                //Go to the Index view (login page)

                //Note: if has authkey then try to get the data
                // the Authentication needs to be run 
                // need to change the case must call Auth API each time and each page. put in GetSessionPreLoadDataAsync().

                if (Authentication.AuthorisationFailed)
                {
                    Response.Cookies.Delete("TP_YCP_1");
                    // Clear all session variables
                    HttpContext.Session.Clear();
                    if (!string.IsNullOrEmpty(WhiteLabelLocation)){
                        ViewBag.WhiteLabel = WhiteLabelLocation;}
                    return View("Index");
                }

                if (WhiteLabelLocation != null)
                {
                    ViewBag.WhiteLabel = WhiteLabelLocation;
                }

                if (Authentication != null)
                {
                    ViewBag.SETTING_StatementsEnabled = Authentication.SETTING_StatementsEnabled;
                    ViewBag.SETTING_BudgetEnabled = Authentication.SETTING_BudgetEnabled;
                    ViewBag.SETTING_SupportplanEnabled = Authentication.SETTING_SupportplanEnabled;
                    ViewBag.SETTING_FormsEnabled = Authentication.SETTING_FormsEnabled;
                    ViewBag.SETTING_NDIS_StatementsEnabled = Authentication.SETTING_NDIS_StatementsEnabled;
                    ViewBag.SETTING_NDIS_Quotes_Enabled = Authentication.SETTING_NDIS_Quotes_Enabled;
                    HttpContext.Session.SetString("PortalUserClientsAdditionalIds", Authentication?.LinkedClientIDS ?? "null");
                }

                if (Request.Query.ContainsKey("Debug"))
                {
                    string debugValue = Request.Query["Debug"].ToString().ToLower();
                    HttpContext.Session.SetString("YCP_Debug", debugValue);
                }

                // InitializeSession Data
                if (HttpContext.Session.GetString("portalUserID") == null)
                {
                    var sessionManager = new SessionManager(_context, _responseHelper, _apiUrls, HttpContext, email);
                    sessionManager.InitializeSession();
                }

                string clientIDs = HttpContext.Session.GetString("PortalUserClientsAdditionalIds") ?? "0";

                if (string.IsNullOrEmpty(clientIDs))
                {
                    ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                    ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                    return View("NoData"); // Consider showing a different message here indicating missing clientIDs
                }

                if (clients == null || !clients.Any())
                {
                    ViewBag.NoDataMessage = _noDataMsg.ClientIDsMainText;
                    ViewBag.NoDataSubText = _noDataMsg.ClientIDsSubText;
                    return View("NoData");
                }

                var (root, debugMessage, jsonString) = await _responseHelper.GetResponse<SupportPlans>(authKey, _apiUrls.SupportPlan);

                var supportPlan = new SupportPlans();

                if (root == null)
                {
                    ViewBag.NoDataMessage = _noDataMsg.BudgetMainText;
                    ViewBag.NoDataSubText = _noDataMsg.BudgetSubText;
                }
                else
                {
                    var obj = JsonConvert.DeserializeObject<dynamic>(jsonString);
                    var planContents = obj.planContents.ToObject<List<SupportPlanPlanItem>>();

                    SupportPlanGoalGroup currentGoal = null;
                    SupportPlanActionGroup currentAction = null;

                    foreach (var item in planContents)
                    {
                        if (item.planItemType == "label_extend_start")
                        {
                            currentGoal = new SupportPlanGoalGroup { Goal = item };
                            supportPlan.Goals.Add(currentGoal);
                            continue;
                        }

                        if (currentGoal == null)
                        {
                            supportPlan.Items.Add(item);
                            continue;
                        }

                        if (item.planItemName == "Actions") continue;

                        if (item.planItemName == "Action")
                        {
                            currentAction = new SupportPlanActionGroup { Action = item };
                            currentGoal.Actions.Add(currentAction);
                            continue;
                        }

                        if (currentAction != null)
                        {
                            if (item.planItemName == "Achieved") { currentAction.Achieved = item; continue; }
                            if (item.planItemName == "Action Date") { currentAction.ActionDate = item; continue; }
                            if (item.planItemName == "Person Responsible") { currentAction.PersonResponsible = item; currentAction = null; continue; }
                        }

                        if (item.planItemType == "label_extend_end")
                        {
                            currentGoal = null;
                            continue;
                        }

                        currentGoal.OtherItems.Add(item);
                    }

                    //Log Page Access
                    ILogAccess logger = new UpdatePortalAccessLog();
                    LogAccessHelper logHelper = new LogAccessHelper(logger, HttpContext);
                    logHelper.LogPageAccess("SupportPlan");

                    var model = new SupportPlansandClients
                    {
                        ClientDetails = clients,
                        SupportPlans = supportPlan
                    };

                    ViewBag.Title = "Support Plan - Your Care Portal";

                    return View("SupportPlan", model);
                }
                return View("NoData");
            }
            catch (Exception ex)
            {
                // Log the error
                TempData["ErrorMessage"] = "An error occurred.";
                return View("Error");  // Assuming you have an error view
            }
        }

        /// <summary>
        /// Asynchronously retrieves and displays the support plan for a specified client.
        /// </summary>
        /// <param name="clientId">The optional ID of the client for whom to retrieve the support plan.</param>
        /// <returns>A PartialViewResult for the support plan if successful, otherwise a Json result indicating an error.</returns>
        [HttpGet]
        public async Task<IActionResult> SupportPlanAction(int? clientId)
        {
            try
            {
                // Retrieve authentication key from cookie
                string jsonString3 = Request.Cookies["TP_YCP_1"] ?? string.Empty;
                var myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);
                var authKey = myObject1?.AuthKey ?? string.Empty;

                // If the authentication key is missing, return to the Index view with an error message
                if (string.IsNullOrEmpty(authKey))
                {
                    TempData["ErrorMessage"] = "Failed to Authenticate";
                    return View("Index");
                }

                // Attempt to fetch data using the Pre Load Service
                var (_, _, clients) = await _sessionPreLoadDataService.GetSessionPreLoadDataAsync();

                var matchingClients = clients.Where(c => clientId.HasValue && c.clientID == clientId.Value).ToList();

                // Construct the URL to fetch client details
                var URL1 = _apiUrls.ClientDetails + "?clientID=" + clientId?.ToString();

                // Retrieve the client details; if failed, return the NoData view
                var (root1, debugMessage1, jsonString1) = await _responseHelper.GetResponse<Root>(authKey, URL1);
                if (root1 == null)
                {
                    return View("NoData");
                }

                if (clients == null || !clients.Any())
                {
                    ViewBag.NoDataMessage = _noDataMsg.MainText;
                    ViewBag.NoDataSubText = _noDataMsg.SubText;
                    return View("NoData");
                }

                // Retrieve the support plan; if failed, display a message
                var (root, debugMessage, jsonString) = await _responseHelper.GetResponse<RootSupportPlanPlan>(authKey, _apiUrls.SupportPlan);
                var supportPlan = new SupportPlans();
                if (root == null)
                {
                    ViewBag.NoDataMessage = _noDataMsg.BudgetMainText;
                    ViewBag.NoDataSubText = _noDataMsg.BudgetSubText;
                }
                else
                {
                    // Deserialize the support plan items from JSON
                    var obj = JsonConvert.DeserializeObject<dynamic>(jsonString);
                    var planContents = obj.planContents.ToObject<List<SupportPlanPlanItem>>();

                    // Process each item in the support plan, organizing them into goals and actions
                    SupportPlanGoalGroup currentGoal = null;
                    SupportPlanActionGroup currentAction = null;
                    foreach (var item in planContents)
                    {
                        // Start a new goal group
                        if (item.planItemType == "label_extend_start")
                        {
                            currentGoal = new SupportPlanGoalGroup { Goal = item };
                            supportPlan.Goals.Add(currentGoal);
                            continue;
                        }

                        // If no current goal, add the item to the standalone items list
                        if (currentGoal == null)
                        {
                            supportPlan.Items.Add(item);
                            continue;
                        }

                        // Skip items that represent action headers
                        if (item.planItemName == "Actions") continue;

                        // Start a new action group within the current goal
                        if (item.planItemName == "Action")
                        {
                            currentAction = new SupportPlanActionGroup { Action = item };
                            currentGoal.Actions.Add(currentAction);
                            continue;
                        }

                        // Add details to the current action
                        if (currentAction != null)
                        {
                            if (item.planItemName == "Achieved") { currentAction.Achieved = item; continue; }
                            if (item.planItemName == "Action Date") { currentAction.ActionDate = item; continue; }
                            if (item.planItemName == "Person Responsible") { currentAction.PersonResponsible = item; currentAction = null; continue; }
                        }

                        // End the current goal group
                        if (item.planItemType == "label_extend_end")
                        {
                            currentGoal = null;
                            continue;
                        }

                        // Add any other items to the current goal's list of other items
                        currentGoal.OtherItems.Add(item);
                    }
                }

                // Construct the model for the partial view
                var model = new SupportPlansandClients
                {
                    ClientDetails = matchingClients,
                    SupportPlans = supportPlan
                };

                // Return the partial view with the model
                return PartialView("SupportPlanPartial", model);
            }
            catch (Exception ex)
            {
                // Log the exception details to the console
                Console.WriteLine(ex);

                // Return a JSON result indicating failure and the exception message
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
