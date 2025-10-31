namespace YourCarePortal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using YourCarePortal.Models;


    /// <summary>
    /// Helps in sending requests to the specified API endpoint and processing the responses.
    /// Specifically designed for operations related to API responses.
    /// </summary>
    public class APIResponseHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="APIResponseHelper"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient used to send requests.</param>
        /// <param name="URL">The API endpoint URL.</param>
        public APIResponseHelper(HttpClient httpClient, string URL)
        {
            _httpClient = httpClient;
            _apiUrl = URL;  // get the ApiUrls from the injected IOptions
        }

        /// <summary>
        /// Sends a GET request to the specified API endpoint to change the password, 
        /// and deserializes the response into a <see cref="RootChangePassword"/> object.
        /// </summary>
        /// <param name="parameters">The parameters to be sent with the request.</param>
        /// <returns>The deserialized <see cref="RootChangePassword"/> object.</returns>
        public async Task<RootChangePassword> ChangePassword(params KeyValuePair<string, string>[] parameters)
        {
            if (_apiUrl == null)
            {
                throw new ArgumentNullException($"Endpoint 'Authenticate' not found in API URLs.");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, _apiUrl);
            var collection = new List<KeyValuePair<string, string>>(parameters);

            // Encode the parameters into the content of the request.
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            HttpResponseMessage response;

            // Attempt to send the request and ensure a successful response.
            try
            {
                response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                // Proper logging can be added here.
                throw new HttpRequestException("There was a problem contacting the authentication service. Please try again later.", e);
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            RootChangePassword result;

            // Attempt to deserialize the response content.
            try
            {
                result = System.Text.Json.JsonSerializer.Deserialize<RootChangePassword>(responseBody);
            }
            catch (System.Text.Json.JsonException e)
            {
                // Proper logging can be added here.
                throw new InvalidOperationException("Failed to deserialize the API response.", e);
            }

            if (result == null)
            {
                throw new NullReferenceException("Deserialized object is null.");
            }

            return result;
        }
    }
}
