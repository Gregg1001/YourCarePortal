using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YourCarePortal.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Microsoft.Extensions.Options;
using YourCarePortal.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;
using System.Globalization;
using YourCarePortal.Services;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Net.Http;
using YourCarePortal.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NUglify.JavaScript.Syntax;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.Xml;

namespace YourCarePortal.Controllers
{
    /// <summary>
    /// Controller responsible for the primary interactions of the home page.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthenticationHelper _authHelper;
        private readonly ChangePasswordHelper _changePasswordHelper;
        private readonly ResponseHelper _responseHelper;
        private readonly ApiUrls _apiUrls;
        private readonly OtherURLs _otherUrls; // Consider injecting or removing if unused.
        private readonly DatabaseContext _context;
        private readonly SessionDataHelper _sessionDataHelper; // Consider injecting or removing if unused.
        private readonly NoDataMsg _noDataMsg;
        private readonly IHttpClientFactory _clientFactory;
        private readonly PasswordResetActionService _passwordResetActionService;
        private readonly IConfiguration _configuration;
        private GetAuthKeyService _getAuthKeyService;
        private IMemoryCache _cache; // Used for memory caching.
        private RootObject _rootObject; // Consider its purpose or remove if unnecessary.
        private readonly GetWhitelabelImageLocation _imageLocationService;

        //private readonly GetAuthKeyService _getAuthKeyService;
        //private readonly ClientDataService _clientDataService;
        //private readonly BudgetDataService _budgetDataService;

        private readonly UrlParameterService _urlParameterService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">Logging service.</param>
        /// <param name="authHelper">Helper service for authentication.</param>
        /// <param name="changePasswordHelper">Helper service for changing password.</param>
        /// <param name="responseHelper">Helper service for API responses.</param>
        /// <param name="apiUrls">API URL configurations.</param>
        /// <param name="noDataMsg">Configuration for no data messages.</param>
        /// <param name="context">Database context.</param>
        /// <param name="memoryCache">Memory caching service.</param>
        /// <param name="clientFactory">Factory to create instances of HttpClient.</param>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="passwordResetActionService">Service for password reset actions.</param>
        public HomeController(
            ILogger<HomeController> logger,
            AuthenticationHelper authHelper,
            ChangePasswordHelper changePasswordHelper,
            ResponseHelper responseHelper,
            IOptions<ApiUrls> apiUrls,
            IOptions<NoDataMsg> noDataMsg,
            DatabaseContext context,
            IMemoryCache memoryCache,
            IHttpClientFactory clientFactory,
            IConfiguration configuration,
            PasswordResetActionService passwordResetActionService,
            GetAuthKeyService getAuthKeyService,
            UrlParameterService urlParameterService)

        {
            _logger = logger;
            _authHelper = authHelper;
            _changePasswordHelper = changePasswordHelper;
            _responseHelper = responseHelper;
            _apiUrls = apiUrls.Value;
            _context = context;
            _cache = memoryCache;
            _noDataMsg = noDataMsg.Value;
            _clientFactory = clientFactory;
            _passwordResetActionService = passwordResetActionService;

            _imageLocationService = new GetWhitelabelImageLocation(configuration);

            _configuration = configuration;
            _urlParameterService = urlParameterService;

            //_getAuthKeyService = getAuthKeyService;
            //_clientDataService = clientDataService;
            //_budgetDataService = budgetDataService;

        }


        /// <summary>
        /// Handles the default view of the home page.
        /// </summary>
        /// <returns>The default view.</returns>
        public async Task<IActionResult> Index()

        {
            ////White Label//
            // Obtain the current URL
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";

            // Fetch the image location using the service
            var WhiteLabelLocation = _imageLocationService.GetImageLocationFromUrl(currentUrl);

            if (WhiteLabelLocation != null)
            {
                ViewBag.WhiteLabel = WhiteLabelLocation;
            }

            return View();
        }

        /// <summary>
        /// Handles the privacy view of the application.
        /// </summary>
        /// <returns>The privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        //IndexResetPassword

        /// <summary>
        /// Asynchronously renders the reset password view for the current user.
        /// </summary>
        /// <remarks>
        /// This method fetches the white label image location based on the current URL
        /// and passes it to the view through the ViewBag. It also sets the ViewBag
        /// for the reset password sequence.
        /// </remarks>
        /// <returns>
        /// The task result contains the action result for rendering the IndexResetPassword view.
        /// </returns>

        public async Task<IActionResult> IndexResetPassword()
        {
            ////White Label//
            // Obtain the current URL
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";

            // Fetch the image location using the service
            var WhiteLabelLocation = _imageLocationService.GetImageLocationFromUrl(currentUrl);

            if (WhiteLabelLocation != null)
            {
                ViewBag.WhiteLabel = WhiteLabelLocation;
            }

            ViewBag.ForgetPasswordSeq = "IndexResetPasswordPartial1";

            //ViewBag.SeqID = "1";

            //return PartialView("IndexResetPasswordPartial-1", new YourCarePortal.Models.RootIndexResetPassword { CurrentStep = "1" });

            // return PartialView("IndexResetPasswordPartial-1", new { CurrentStep = "1" });

            return View("IndexResetPassword", new RootIndexResetPassword { CurrentStep = "1" });
        }



        /// <summary>
        /// Logs out the user by clearing cookies and session data.
        /// </summary>
        /// <returns>A redirect action to the Index page.</returns>
        public IActionResult Logout()
        {
            // Delete the authentication cookie
            Response.Cookies.Delete("TP_YCP_1");

            // Clear all session variables
            HttpContext.Session.Clear();
            //gsstop

            // Redirect to the Index page
            return RedirectToAction("Index");
        }

        public IActionResult BacktoLogin()
        {
            // Delete the authentication cookie
            Response.Cookies.Delete("TP_YCP_1");

            // Clear all session variables
            HttpContext.Session.Clear();
            //gsstop

            // Redirect to the Index page
            return RedirectToAction("Index");
        }


        [HttpPost]

        public async Task<IActionResult> IndexResetPasswordAction(string currentStep, string tempCode, string email, string newPassword, string tempCode_hidden)
        {

            //Start the Sequence
            if (currentStep == null)
            {
                currentStep = "1";
            };

            var (model, success) = await _passwordResetActionService.ProcessResetPasswordAction(currentStep, tempCode, email, newPassword, tempCode_hidden);
            if (success)
            {
                ////White Label//
                ///
                // Obtain the current URL
                string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";

                // Fetch the image location using the service
                var WhiteLabelLocation = _imageLocationService.GetImageLocationFromUrl(currentUrl);

                if (WhiteLabelLocation != null)
                {
                    ViewBag.WhiteLabel = WhiteLabelLocation;
                }


                ViewBag.Email = email;

                return View("IndexResetPassword", model);
            }
            else
                return BadRequest("Invalid Reset Password step");
        }

        private IActionResult HandleError(string step, string errorMessage)
        {
            TempData["ErrorMessage"] = errorMessage;
            ViewBag.SeqID = step;
            ViewBag.TempCode = "3333"; // Assuming this is common for all errors

            ViewBag.ForgetPasswordSeq = $"IndexResetPasswordPartial{step}";
            //ViewBag.ErrorandMsg = $"Error in step {step}: {errorMessage}";

            ViewBag.ErrorandMsg = $"ErrorGregg";

            // Console.WriteLine($"Error in step {step}: {errorMessage}");
            return PartialView($"IndexResetPasswordPartial{step}");
        }

    }
}

