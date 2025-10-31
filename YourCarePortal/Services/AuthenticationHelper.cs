using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using YourCarePortal.Models;

/// <summary>
/// Helps in authenticating users using different methods such as email-password and authentication key.
/// </summary>
public class AuthenticationHelper
{
    private readonly HttpClient _httpClient;
    private readonly ApiUrls _apiUrls;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationHelper"/> class.
    /// </summary>
    /// <param name="httpClient">The HttpClient used for making HTTP requests.</param>
    /// <param name="apiUrls">The API URLs configuration.</param>
    public AuthenticationHelper(HttpClient httpClient, IOptions<ApiUrls> apiUrls)
    {
        _httpClient = httpClient;
        _apiUrls = apiUrls.Value;
    }

    /// <summary>
    /// Authenticate a user using their email and password.
    /// </summary>
    /// <param name="email">User's email.</param>
    /// <param name="password">User's password.</param>
    /// <returns>Authorisation details.</returns>
    public async Task<Authorisation> Authenticate(string email, string password)
    {
        return await AuthenticateInternal("email_password", email, password, null);
    }

    /// <summary>
    /// Authenticate a user using their authentication key.
    /// </summary>
    /// <param name="authKey">Authentication key.</param>
    /// <returns>Authorisation details.</returns>
    public async Task<Authorisation> Authenticate(string authKey)
    {
        return await AuthenticateInternal("auth_key", null, null, authKey);
    }

    /// <summary>
    /// Internal method to authenticate using the specified method and details.
    /// </summary>
    /// <param name="authType">Type of authentication (e.g., "email_password" or "auth_key").</param>
    /// <param name="email">User's email, if authType is "email_password".</param>
    /// <param name="password">User's password, if authType is "email_password".</param>
    /// <param name="authKey">Authentication key, if authType is "auth_key".</param>
    /// <returns>Authorisation details.</returns>
    private async Task<Authorisation> AuthenticateInternal(string authType, string email, string password, string authKey)
    {
        var url = _apiUrls.Authenticate;

        if (url == null)
        {
            throw new ArgumentNullException($"Endpoint 'Authenticate' not found in API URLs.");
        }

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var collection = new List<KeyValuePair<string, string>>();

        // Construct the request content based on the authentication type.
        if (authType == "email_password")
        {
            collection.Add(new KeyValuePair<string, string>("email", email));
            collection.Add(new KeyValuePair<string, string>("password", password));
        }
        else if (authType == "auth_key")
        {
            collection.Add(new KeyValuePair<string, string>("authKey", authKey));
        }

        var content = new FormUrlEncodedContent(collection);
        request.Content = content;

        HttpResponseMessage response;
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

        Authorisation authorisation;
        try
        {
            authorisation = System.Text.Json.JsonSerializer.Deserialize<Authorisation>(responseBody);
        }
        catch (System.Text.Json.JsonException e)
        {
            // Proper logging can be added here.
            throw new InvalidOperationException("Failed to deserialize the API response.", e);
        }

        if (authorisation?.AUTHKEY == null)
        {
            // throw new NullReferenceException("Authentication failed, returned object or AUTHKEY is null.");
            authorisation.AuthorisationFailed = true;

            // ### re-create Auth Key and try again using the email and password (read in) in the cookie.
            // or easier to go to the Login page? yes keep it simple!!!

            // How to redirect to the Login page? If Error then redirect to the Login page.

        }

        return authorisation;
    }
}
