namespace YourCarePortal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;

    using Newtonsoft.Json;

    /// <summary>
    /// Helps in sending requests to the specified API endpoint and processing the responses.
    /// </summary>
    public class AllAPIResponseHelper
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllAPIResponseHelper"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient used to send requests.</param>
        /// <param name="URL">The API endpoint URL.</param>
        public AllAPIResponseHelper(HttpClient httpClient, string URL)
        {
            _httpClient = httpClient;
            _apiUrl = URL;
        }

        /// <summary>
        /// Sends a GET request to the specified API endpoint with the provided parameters and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The type to which the response needs to be deserialized.</typeparam>
        /// <param name="parameters">The parameters to be sent with the request.</param>
        /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
        public async Task<T> SendRequest<T>(params KeyValuePair<string, string>[] parameters)
        {
            if (_apiUrl == null)
            {
                throw new ArgumentNullException($"Endpoint not found in API URLs.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl);
            var collection = new List<KeyValuePair<string, string>>(parameters);

            // Encode the parameters into the content of the request.
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            HttpResponseMessage response;

            //??
            // Set the MaxResponseContentBufferSize
            // Example: Setting to 5GB
            
            //_httpClient.MaxResponseContentBufferSize = 1L * 1024L * 1024L * 1024L;


            // Attempt to send the request and ensure a successful response.
            try
            {
                response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                // Proper logging can be added here.
                throw new HttpRequestException("There was a problem contacting the service. Please try again later.", e);
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            //{"status":"ERR_SIGNED","message":"Quote already signed by client [provider: 1 client:13692 quote:15]"}

            T result;

            // Attempt to deserialize the response content.
            try
            {
                result = System.Text.Json.JsonSerializer.Deserialize<T>(responseBody);
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
