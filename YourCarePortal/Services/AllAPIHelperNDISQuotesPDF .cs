namespace YourCarePortal.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Helps in sending requests to the specified API endpoint and processing PDF responses.
    /// </summary>
    public class AllAPIHelperNDISQuotesPDF
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllAPIHelperNDISQuotesPDF"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient used to send requests.</param>
        /// <param name="URL">The API endpoint URL.</param>
        public AllAPIHelperNDISQuotesPDF(HttpClient httpClient, string URL)
        {
            _httpClient = httpClient;
            _apiUrl = URL;
        }

        /// <summary>
        /// Sends a request to the specified API endpoint and retrieves a PDF response.
        /// </summary>
        /// <param name="parameters">The parameters to be sent with the request.</param>
        /// <returns>A byte array containing the PDF data.</returns>
        public async Task<byte[]> SendRequestForPdf(params KeyValuePair<string, string>[] parameters)
        {
            if (string.IsNullOrEmpty(_apiUrl))
            {
                throw new ArgumentNullException(nameof(_apiUrl), "API URL cannot be null or empty.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl);
            request.Content = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                throw new HttpRequestException("There was a problem contacting the service.", e);
            }

            // Expecting the response to be a PDF file
            if (response.Content.Headers.ContentType.MediaType == "application/pdf")
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                throw new InvalidOperationException("The response content is not a PDF.");
            }
        }

        internal Task SendRequestForPdf<T>(KeyValuePair<string, string> keyValuePair1, KeyValuePair<string, string> keyValuePair2, KeyValuePair<string, string> keyValuePair3)
        {
            throw new NotImplementedException();
        }
    }
}
