using Microsoft.AspNetCore.Http;
using YourCarePortal.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using YourCarePortal.Data;
using Microsoft.Data.SqlClient;
using System;

namespace YourCarePortal.Services
{
    /// <summary>
    /// Provides methods to retrieve providers data from the database.
    /// </summary>
    public class ProviderDataHelper
    {
        private readonly DatabaseContext _context;
        private readonly ResponseHelper _responseHelper;
        private readonly ApiUrls _apiUrls;
        private readonly HttpContext _httpContextAccessor;

        /// <summary>
        /// Gets a value indicating whether this instance has been successfully initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderDataHelper"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="responseHelper">The response helper.</param>
        /// <param name="apiUrls">The API URLs configuration.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public ProviderDataHelper(
            DatabaseContext context,
            ResponseHelper responseHelper,
            ApiUrls apiUrls,
            HttpContext httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _responseHelper = responseHelper ?? throw new ArgumentNullException(nameof(responseHelper));
            _apiUrls = apiUrls ?? throw new ArgumentNullException(nameof(apiUrls));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            // Assuming all these services are essential for the proper working of this class,
            // so if any of them is null, IsInitialized will be set to false.
            IsInitialized = _context != null && _responseHelper != null && _apiUrls != null && _httpContextAccessor != null;
        }

        /// <summary>
        /// Retrieves a list of providers based on client IDs.
        /// </summary>
        /// <param name="clientIds">A comma-separated string of client IDs.</param>
        /// <returns>A list of <see cref="ProviderDetails"/> objects.</returns>
        public async Task<string> GetProvidersByClientIds(string clientIds)
        {
            var ids = clientIds.Split(',').Select(int.Parse).ToList();
            var parameters = ids.Select((id, index) => new SqlParameter($"@id{index}", id)).ToArray();
            var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));

            string sqlQuery = $"SELECT [clientCompanyID] FROM [tblClients] WHERE [ClientId] IN ({parameterNames});";

            try
            {
                var queryResult = await _context.QueryProviderID
                    .FromSqlRaw(string.Format(sqlQuery, parameterNames), parameters)
                    .ToListAsync();

                if (!queryResult.Any())
                {
                    // Return an empty string or handle as needed.
                    return string.Empty;
                }

                // Assuming the objects in queryResult have a property named 'ProviderId' that you want to include in the CSV.
                var csvResult = string.Join(",", queryResult.Select(x => x.clientCompanyID));
                return csvResult;
            }
            catch (Exception e)
            {
                // Log the exception here.
                return null; // Or handle the exception as required.
            }
        }

        // ... Any additional methods or properties ...
    }
}
























