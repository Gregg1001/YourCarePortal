using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using YourCarePortal.Data;
using YourCarePortal.Models;

namespace YourCarePortal.Services
{
    /// <summary>
    /// Provides methods to retrieve client data from the database.
    /// </summary>
    public class ClientDataHelperNew
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<ClientDataHelperNew> _logger;
        private readonly ApiUrls _apiUrls;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientDataHelper"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger for logging information and errors.</param>
        /// <param name="apiUrls">The API URLs configuration.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public ClientDataHelperNew(
            DatabaseContext context,
            ILogger<ClientDataHelperNew> logger,
            ApiUrls apiUrls,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiUrls = apiUrls ?? throw new ArgumentNullException(nameof(apiUrls));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Retrieves a list of clients by their IDs.
        /// </summary>
        /// <param name="clientIds">A comma-separated string of client IDs.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="QueryClientDetails"/> objects.</returns>
        public async Task<List<QueryClientDetails>> GetClientByIds(string clientIds)
        {
            if (string.IsNullOrWhiteSpace(clientIds))
            {
                _logger.LogError("Client IDs were null or whitespace.");
                return new List<QueryClientDetails>();
            }

            var ids = clientIds.Split(',').Select(int.Parse).ToList();
            var parameters = ids.Select((id, index) => new SqlParameter($"@Id{index}", id)).ToArray();
            var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));

            var sqlClient = $@"SELECT [clientID], [clientFirstname], [clientSurname], [clientAddress], [clientSuburb], 
                                [clientState], [clientPostcode], [clientFullName], [clientCaseManager], uS.[userFirstName], 
                                US.[userSurname], [ClientCompanyID], [companyName], [companyPhone], [companyAddress], 
                                [companySuburb], [companyPostcode], [companyState], [companyLogo], [clientPhotoPath1], 
                                [companyABN] FROM[tblClients] cl INNER JOIN[dbo].[tblCompanies] co ON cl.[clientCompanyID] 
                                = co.[companyID] LEFT JOIN [dbo].[tblUsers] us ON cl.[clientCaseManager] = us.[userID] 
                                WHERE [clientID] IN ({parameterNames});";

            try
            {
                var queryResult = await _context.QueryClientDetails.FromSqlRaw(sqlClient, parameters).ToListAsync();

                return queryResult;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving client details by IDs.");
                throw; // Or return an empty list/new List<QueryClientDetails>() depending on how you want to handle errors.
            }
        }
    }
}
