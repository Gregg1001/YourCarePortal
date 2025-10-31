using Newtonsoft.Json;
using System;
using YourCarePortal.Models;

namespace YourCarePortal.Services
{
    /// <summary>
    /// Service to retrieve the image location for a white-labeled client based on the URL provided.
    /// </summary>
    public class GetWhitelabelImageLocation
    {
        private readonly List<WhiteLabel> _appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWhitelabelImageLocation"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration where white label settings are stored.</param>
        public GetWhitelabelImageLocation(IConfiguration configuration)
        {
            _appSettings = configuration.GetSection("WhiteLabelSettings").Get<List<WhiteLabel>>();
        }

        /// <summary>
        /// Gets the image location for a white-labeled client based on the domain name extracted from the provided URL.
        /// </summary>
        /// <param name="fullUrl">The full URL to process for domain name extraction.</param>
        /// <returns>The image location as a string if a matching setting is found; otherwise, null.</returns>
        /// <exception cref="InvalidOperationException">Thrown when application settings are not initialized.</exception>
        public string GetImageLocationFromUrl(string fullUrl)
        {
            if (string.IsNullOrEmpty(fullUrl))
                return null;

            string domainToSearch = URLUtility.GetDomainName(fullUrl);

            if (_appSettings == null)
                throw new InvalidOperationException("App settings not initialized.");

            var matchingSetting = _appSettings.FirstOrDefault(s => s.DomainName.Equals(domainToSearch, StringComparison.OrdinalIgnoreCase));

            return matchingSetting?.ImageLocation;
        }
    }
}
