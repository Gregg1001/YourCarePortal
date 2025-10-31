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
using System.Globalization;
using YourCarePortal.Services;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Net.Http;
using YourCarePortal.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.Xml;

namespace YourCarePortal.Controllers
{
    [NoCache]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthenticationHelper _authHelper;
        private readonly ChangePasswordHelper _changePasswordHelper;
        private readonly ResponseHelper _responseHelper;
        private readonly ApiUrls _apiUrls;
        private readonly DatabaseContext _context;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly PasswordResetActionService _passwordResetActionService;
        private readonly GetAuthKeyService _getAuthKeyService;
        private readonly GetWhitelabelImageLocation _imageLocationService;
        private readonly UrlParameterService _urlParameterService;

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
            _clientFactory = clientFactory;
            _passwordResetActionService = passwordResetActionService;
            _configuration = configuration;
            _imageLocationService = new GetWhitelabelImageLocation(configuration);
            _getAuthKeyService = getAuthKeyService;
            _urlParameterService = urlParameterService;
        }

        public async Task<IActionResult> Index()
        {
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            var WhiteLabelLocation = _imageLocationService.GetImageLocationFromUrl(currentUrl);

            if (WhiteLabelLocation != null)
            {
                ViewBag.WhiteLabel = WhiteLabelLocation;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> IndexResetPassword()
        {
            string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            var WhiteLabelLocation = _imageLocationService.GetImageLocationFromUrl(currentUrl);

            if (WhiteLabelLocation != null)
            {
                ViewBag.WhiteLabel = WhiteLabelLocation;
            }

            ViewBag.ForgetPasswordSeq = "IndexResetPasswordPartial1";
            return View("IndexResetPassword", new RootIndexResetPassword { CurrentStep = "1" });
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("TP_YCP_1");
            HttpContext.Session.Clear();

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0, private";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";

            return RedirectToAction("Index");
        }

        public IActionResult BacktoLogin()
        {
            Response.Cookies.Delete("TP_YCP_1");
            HttpContext.Session.Clear();

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0, private";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "-1";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> IndexResetPasswordAction(string currentStep, string tempCode, string email, string newPassword, string tempCode_hidden)
        {
            if (currentStep == null)
            {
                currentStep = "1";
            }

            var (model, success) = await _passwordResetActionService.ProcessResetPasswordAction(currentStep, tempCode, email, newPassword, tempCode_hidden);
            if (success)
            {
                string currentUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
                var WhiteLabelLocation = _imageLocationService.GetImageLocationFromUrl(currentUrl);

                if (WhiteLabelLocation != null)
                {
                    ViewBag.WhiteLabel = WhiteLabelLocation;
                }

                ViewBag.Email = email;
                return View("IndexResetPassword", model);
            }
            else
            {
                return BadRequest("Invalid Reset Password step");
            }
        }

        private IActionResult HandleError(string step, string errorMessage)
        {
            TempData["ErrorMessage"] = errorMessage;
            ViewBag.SeqID = step;
            ViewBag.TempCode = "3333";

            ViewBag.ForgetPasswordSeq = $"IndexResetPasswordPartial{step}";
            ViewBag.ErrorandMsg = "ErrorGregg";

            return PartialView($"IndexResetPasswordPartial{step}");
        }
    }
}
