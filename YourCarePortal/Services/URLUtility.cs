namespace YourCarePortal.Services
{
    /// <summary>
    /// Provides utility functions for processing URLs.
    /// </summary>
    public class URLUtility
    {
        /// <summary>
        /// Retrieves the domain name from the given URL.
        /// </summary>
        /// <param name="url">The URL from which to extract the domain name.</param>
        /// <returns>The domain name as a string.</returns>
        /// <exception cref="System.UriFormatException">Thrown when the URL is not in a correct format.</exception>
        public static string GetDomainName(string url)
        {
            try
            {
                Uri parsedUri = new Uri(url);
                return parsedUri.Host;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
