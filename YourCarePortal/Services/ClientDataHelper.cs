using Microsoft.AspNetCore.Http;
using YourCarePortal.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using YourCarePortal.Data;
using System.Data;
using Microsoft.Data.SqlClient;
using static Humanizer.In;
using System.ComponentModel.Design;

namespace YourCarePortal.Services
{
    /// <summary>
    /// Provides methods to retrieve client data from the database.
    /// </summary>
    public class ClientDataHelper
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
        /// Initializes a new instance of the <see cref="ClientDataHelper"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="responseHelper">The response helper.</param>
        /// <param name="apiUrls">The API URLs configuration.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public ClientDataHelper(
            DatabaseContext context,
            ResponseHelper responseHelper,
            ApiUrls apiUrls,
            HttpContext httpContextAccessor)
        {
            _context = context;
            _responseHelper = responseHelper;
            _apiUrls = apiUrls;
            _httpContextAccessor = httpContextAccessor;

            // Assuming all these services are essential for the proper working of this class,
            // so if any of them is null, IsInitialized will be set to false.
            IsInitialized = _context != null && _responseHelper != null && _apiUrls != null && _httpContextAccessor != null;
        }

        /// <summary>
        /// Retrieves a list of clients by their IDs.
        /// </summary>
        /// <param name="clientIds">A comma-separated string of client IDs.</param>
        /// <returns>A list of <see cref="QueryClientDetails"/> objects.</returns>
        public async Task<List<QueryClientDetails>> GetClientByIds(string clientIds)
        {
            var ids = clientIds.Split(',').Select(int.Parse).ToList();

            // SQL query with placeholders for parameterized input to prevent SQL injection.
            string sqlClient = @"SELECT [clientID], [clientFirstname], [clientSurname], [clientAddress], [clientSuburb],[clientState], [clientPostcode], [clientFullName], [clientCaseManager], uS.[userFirstName], US.[userSurname],[ClientCompanyID]
                                , [companyName], [companyPhone], [companyAddress], [companySuburb], [companyPostcode], [companyState], [companyLogo], [clientPhotoPath1], [companyABN]
                                FROM[tblClients] cl
                                INNER JOIN[dbo].[tblCompanies] co ON cl.[clientCompanyID] = co.[companyID]
                                LEFT JOIN [dbo].[tblUsers] us ON cl.[clientCaseManager] = [userID]
                                WHERE [clientID] IN ({0});";

            // Construct the SQL IN clause parameters dynamically.
            var parameters = ids.Select((id, index) => new SqlParameter($"@Id{index}", id)).ToArray();
            var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));

            try
            {
                var queryResult = _context.QueryClientDetails
                .FromSqlRaw(string.Format(sqlClient, parameterNames), parameters)
                .ToList();

                if (!queryResult.Any())
                {
                    // Handle the scenario where no records are returned, if needed.
                }

                return queryResult.Select(c => new QueryClientDetails
                {
                    clientID = c.clientID,
                    clientFirstname = c.clientFirstname,
                    clientSurname = c.clientSurname,
                    clientAddress = c.clientAddress,
                    clientSuburb = c.clientSuburb,
                    clientState = c.clientState,
                    clientPostcode = c.clientPostcode,
                    clientFullName = c.clientFullName,
                    clientCaseManager = c.clientCaseManager,
                    userFirstName = c.userFirstName,
                    userSurname = c.userSurname,
                    clientCompanyID = c.clientCompanyID,
                    companyName = c.companyName,
                    companyPhone = c.companyPhone,
                    companyAddress = c.companyAddress,
                    companySuburb = c.companySuburb,
                    companyPostcode = c.companyPostcode,
                    companyState = c.companyState,
                    companyLogo = c.companyLogo,
                    clientPhotoPath1 = c.clientPhotoPath1,
                    companyABN = c.companyABN
                }).ToList();
            }
            catch (Exception e)
            {
                // Maybe log the exception here.
                return null;
            }
        }
    }
}
