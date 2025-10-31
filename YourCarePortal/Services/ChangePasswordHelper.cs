using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using YourCarePortal.Models;

/// <summary>
/// Provides methods to help with changing the user's password.
/// </summary>
public class ChangePasswordHelper
{
    private readonly HttpClient _httpClient;
    private readonly ApiUrls _apiUrls;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangePasswordHelper"/> class.
    /// </summary>
    /// <param name="httpClient">The HttpClient used for making HTTP requests.</param>
    /// <param name="apiUrls">The API URLs configuration.</param>
    public ChangePasswordHelper(HttpClient httpClient, IOptions<ApiUrls> apiUrls)
    {
        _httpClient = httpClient;
        _apiUrls = apiUrls.Value;
    }

    /// <summary>
    /// Changes the user's password using the provided authentication key and new passwords.
    /// </summary>
    /// <param name="Authkey">The authentication key for the user.</param>
    /// <param name="password_change1">The new password.</param>
    /// <param name="password_change2">Confirmation for the new password.</param>
    /// <returns>A <see cref="RootChangePassword"/> object containing the result of the change password operation.</returns>
    public async Task<RootChangePassword> ChangePassword(string Authkey, string password_change1, string password_change2)
    {
        var url = _apiUrls.ChangePassword;
        if (url == null)
        {
            throw new ArgumentNullException($"Endpoint 'ChangePassword' not found in API URLs.");
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Construct the request content with the provided parameters.
        var collection = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("authKey", Authkey),
            new KeyValuePair<string, string>("password_change1", password_change1),
            new KeyValuePair<string, string>("password_change2", password_change2)
        };

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

        RootChangePassword rootChangePassword;

        // Attempt to deserialize the response content.
        try
        {
            rootChangePassword = System.Text.Json.JsonSerializer.Deserialize<RootChangePassword>(responseBody);
        }
        catch (System.Text.Json.JsonException e)
        {
            // Proper logging can be added here.
            throw new InvalidOperationException("Failed to deserialize the API response.", e);
        }

        if (rootChangePassword == null)
        {
            throw new NullReferenceException("Change password failed, deserialized object is null.");
        }

        return rootChangePassword;
    }
}
