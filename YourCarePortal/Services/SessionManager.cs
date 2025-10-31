namespace YourCarePortal.Services
{
    using Azure.Core;
    using global::YourCarePortal.Data;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Manages user sessions within the YourCarePortal application.
    /// </summary>
    /// <remarks>
    /// This service is responsible for initializing user sessions with the necessary session data and handles the retrieval of user emails from cookies.
    /// </remarks>
    public class SessionManager
    {
        private readonly DatabaseContext _context;
        private readonly ResponseHelper _responseHelper;
        private readonly ApiUrls _apiUrls;
        private readonly HttpContext _httpContext;
        private readonly string _email;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionManager"/> class.
        /// </summary>
        /// <param name="context">Database context for data operations.</param>
        /// <param name="responseHelper">Helper class for creating API responses.</param>
        /// <param name="apiUrls">Class containing API URLs for the application.</param>
        /// <param name="httpContext">Current HTTP context for request and response manipulation.</param>
        /// <param name="email">User email to associate with the session.</param>
        public SessionManager(DatabaseContext context, ResponseHelper responseHelper, ApiUrls apiUrls, HttpContext httpContext, string email)
        {
            _context = context;
            _responseHelper = responseHelper;
            _apiUrls = apiUrls;
            _httpContext = httpContext;
            _email = email;
        }

        /// <summary>
        /// Initializes the user session by setting up required session data.
        /// </summary>
        /// <remarks>
        /// This method sets up the session data if not already initialized. It uses a helper class to manage session data operations.
        /// </remarks>
        public void InitializeSession()
        {
            var helper = new SessionDataHelper(_context, _responseHelper, _apiUrls, _httpContext, _email);
            if (helper.IsInitialized)
            {
                // Initialize appointment session with the provided email.
                helper.InitializeAppointmentSession(_email);
            }

            var helper2 = new SessionDataHelper(_context, _responseHelper, _apiUrls, _httpContext, _email);
            try
            {
                // Retrieve email from the cookie and initialize the appointment session.
                string email = GetEmailFromCookie();
                if (helper2.IsInitialized)
                {
                    helper2.InitializeAppointmentSession(email);
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed. You can replace this with your actual logging mechanism.
                Console.WriteLine($"JSON Deserialization failed: {ex.Message}");
                // Consider rethrowing the exception or handling it as per your error handling policy.
            }
        }

        /// <summary>
        /// Retrieves the email of the user from the browser cookies.
        /// </summary>
        /// <returns>
        /// The email address of the user if found in the cookie; otherwise, an empty string.
        /// </returns>
        /// <remarks>
        /// This method attempts to extract the user email from the 'TP_YCP_1' cookie.
        /// </remarks>
        private string GetEmailFromCookie()
        {
            string jsonString = _httpContext.Request.Cookies["TP_YCP_1"] ?? string.Empty;
            var cookieData = JsonConvert.DeserializeObject<YourCarePortal.Models.Cookie>(jsonString);
            return cookieData?.portalUserEmail ?? string.Empty;
        }
    }
}
