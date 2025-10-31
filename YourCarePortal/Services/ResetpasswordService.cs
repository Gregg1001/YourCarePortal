namespace YourCarePortal.Services
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using YourCarePortal.Models;
    public class PasswordResetService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApiUrls _apiUrls;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PasswordResetService(IHttpClientFactory clientFactory, IOptions<ApiUrls> apiUrls, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _apiUrls = apiUrls.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RootIndexResetPassword> StartPasswordResetSequence(string email, string currentStep)
        {

            // Create a HttpClient instance to make HTTP requests
            var httpClient = _clientFactory.CreateClient();

            // Initialize the helper with the necessary dependencies
            var helper = new AllAPIResponseHelper(httpClient, _apiUrls.IndexResetPassword);

            // Variable to hold the password reset sequence response
            var ForgotPasswordSequence = (RootIndexResetPassword)null;

            //---------------------------------------------------------------------------------------------------------------------
            // Step 1: Start the password reset sequence
            // Get the email

            // Send a request to initiate the password reset process
            ForgotPasswordSequence = await helper.SendRequest<RootIndexResetPassword>(
                new KeyValuePair<string, string>("CurrentStep", "1"),
                new KeyValuePair<string, string>("email", email)
            );

            var Status = ForgotPasswordSequence.status;
            var tempCodeHidden = ForgotPasswordSequence.tempCode_hidden;

            // Assuming you have some logic to determine the status, temp code, etc.
            // For now, we'll just instantiate a new RootIndexResetPassword object
            var result = new RootIndexResetPassword
            {
                // Set the properties as needed
                //SeqID = seqId,
                // ... set other properties based on your logic

                status = Status,
                message = ForgotPasswordSequence.message,
                tempCode_hidden = ForgotPasswordSequence.tempCode_hidden
            };

            // Return the RootIndexResetPassword object with SeqID set
            return result;
        }

        public async Task<RootIndexResetPassword> ValidateTemporaryCode(string email, string tempCode, string tempCode_hidden)
        {
            // Implementation of validating the temporary code
            // TODO: Add your logic here

            var CurrentStep = "3";

            // Create a HttpClient instance to make HTTP requests
            var httpClient = _clientFactory.CreateClient();

            // Initialize the helper with the necessary dependencies
            var helper = new AllAPIResponseHelper(httpClient, _apiUrls.IndexResetPassword);

            // Variable to hold the password reset sequence response
            var ForgotPasswordSequence = (RootIndexResetPassword)null;

            // Send a request to validate the temporary code
            var ForgotPasswordSequence2 = await helper.SendRequest<RootIndexResetPassword>(
                new KeyValuePair<string, string>("CurrentStep", "2"),
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("tempCode", tempCode),
                new KeyValuePair<string, string>("tempCode_hidden", tempCode_hidden)
            );

            //ForgotPasswordSequence.tempCode_hidden

            var result = new RootIndexResetPassword
            {
                // Set the properties as needed
                //SeqID = seqId,
                // ... set other properties based on your logic

                status = ForgotPasswordSequence2.status,
                message = ForgotPasswordSequence2.message

            };
            return result;
        }

        public async Task<RootIndexResetPassword> FinalizePasswordReset(string email, string tempCode, string NewPassword, string tempCode_hidden)
        {
            // Implementation of finalizing the password reset

            var CurrentStep = "3";
       
            // Create a HttpClient instance to make HTTP requests
            var httpClient = _clientFactory.CreateClient();

            // Initialize the helper with the necessary dependencies
            var helper = new AllAPIResponseHelper(httpClient, _apiUrls.IndexResetPassword);

            // Variable to hold the password reset sequence response
            var ForgotPasswordSequence = (RootIndexResetPassword)null;

            var ForgotPasswordSequence3 = await helper.SendRequest<RootIndexResetPassword>(
            new KeyValuePair<string, string>("CurrentStep", "3"),
            new KeyValuePair<string, string>("email", email),
            new KeyValuePair<string, string>("tempCode", tempCode),
            new KeyValuePair<string, string>("tempCode_hidden", tempCode_hidden),
            new KeyValuePair<string, string>("password_new", NewPassword) // The new password to be set
);
            var result = new RootIndexResetPassword
            {
                // Set the properties as needed
                //SeqID = seqId,
                // ... set other properties based on your logic

                status = ForgotPasswordSequence3.status,
                message = ForgotPasswordSequence3.message
            };
            // Set properties on the result as needed based on your logic
            return result;
        }
    }
}
