using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Provides methods to handle HTTP responses.
/// </summary>
public class ResponseHelper
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseHelper"/> class.
    /// </summary>
    /// <param name="httpClient">The HttpClient used for making HTTP requests.</param>
    public ResponseHelper(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient), "HttpClient cannot be null.");
    }

    /// <summary>
    /// Sends a GET request with provided parameters and deserializes the response.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize the response to.</typeparam>
    /// <param name="authKey">The authentication key.</param>
    /// <param name="URL">The endpoint URL.</param>
    /// <returns>A tuple containing the deserialized object, a message indicating success or failure, and the response body.</returns>
    public async Task<(T, string, string)> GetResponse<T>(string authKey, string URL)
    {
        if (string.IsNullOrEmpty(authKey))
        {
            throw new ArgumentException("AuthKey cannot be null or empty.", nameof(authKey));
        }

        if (string.IsNullOrEmpty(URL))
        {
            throw new ArgumentException("URL cannot be null or empty.", nameof(URL));
        }

        var requestSchedule = new HttpRequestMessage(HttpMethod.Get, URL);
        var collectionSchedule = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("authkey", authKey)
        };

        var content = new FormUrlEncodedContent(collectionSchedule);
        requestSchedule.Content = content;

        try
        {
            var response = await _httpClient.SendAsync(requestSchedule);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var dataObject = JsonConvert.DeserializeObject<T>(responseBody);

            return (dataObject, "API Response: Success", responseBody);
        }
        catch (Exception e)
        {
            // Return null, the exception message, and the response body if any error occurs
            return (default(T), $"API Response Failure: {e.Message}", null);
        }
    }
}
