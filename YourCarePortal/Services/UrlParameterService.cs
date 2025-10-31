namespace YourCarePortal.Services
{
    /// <summary>
    /// Service to extract parameters from URLs.
    /// </summary>
    public class UrlParameterService
    {
        /// <summary>
        /// Extracts an integer parameter from the given URL.
        /// </summary>
        /// <remarks>
        /// This method splits the URL by '/' and attempts to parse the last segment as an integer.
        /// </remarks>
        /// <param name="url">The URL from which to extract the parameter.</param>
        /// <returns>
        /// The extracted parameter as an integer. If the parameter is not found or is not an integer, returns -1.
        /// </returns>
        public int GetParameterFromUrl(string url)
        {
            // Split the URL and extract the parameter
            var parts = url.Split('/');
            if (parts.Length > 0 && int.TryParse(parts[^1], out int param))
            {
                return param;
            }
            return -1; // Return -1 or appropriate error value if parameter is not found or invalid
        }
    }

}
