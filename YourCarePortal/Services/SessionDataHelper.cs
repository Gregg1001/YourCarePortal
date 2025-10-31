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

namespace YourCarePortal.Services
{
    /// <summary>
    /// Helper class for managing client session data.
    /// </summary>
    public class SessionDataHelper
    {
        private readonly DatabaseContext _context;
        private readonly ResponseHelper _responseHelper;
        private readonly ApiUrls _apiUrls;
        private readonly HttpContext _httpContextAccessor;

        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionDataHelper"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="responseHelper">The helper to handle API responses.</param>
        /// <param name="apiUrls">The API URLs configuration.</param>
        /// <param name="httpContextAccessor">Provides access to the current HttpContext.</param>
        public SessionDataHelper(
            DatabaseContext context,
            ResponseHelper responseHelper,
            ApiUrls apiUrls,
            HttpContext httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _responseHelper = responseHelper ?? throw new ArgumentNullException(nameof(responseHelper));
            _apiUrls = apiUrls ?? throw new ArgumentNullException(nameof(apiUrls));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            // If all essential services are initialized properly, IsInitialized is set to true.
            IsInitialized = true;
        }

        public SessionDataHelper(DatabaseContext context, ResponseHelper responseHelper, ApiUrls apiUrls, HttpContext httpContextAccessor, string email) : this(context, responseHelper, apiUrls, httpContextAccessor)
        {
        }

        /// <summary>
        /// Initializes the appointment session.
        /// </summary>
        /// <param name="Email1">The email of the portal user.</param>
        /// <returns>A <see cref="SessionInitializationResult"/> object indicating whether the initialization was successful.</returns>
        public async Task<SessionInitializationResult> InitializeAppointmentSession(string Email1)
        {
            if (string.IsNullOrEmpty(Email1))
            {
                //### here
                //throw new ArgumentException("Email cannot be null or empty.", nameof(Email1));
                return new SessionInitializationResult { Success = true };
            }

            string jsonString3 = _httpContextAccessor.Request.Cookies["TP_YCP_1"] ?? string.Empty;

            // Deserialize the cookie data if needed.
            // var myObject1 = JsonConvert.DeserializeObject<Cookie>(jsonString3);

            List<PortalUserType> PortalUser = await GetPortalUserByEmail(Email1);

            if (PortalUser.Any())
            {
                PortalUserType item = PortalUser.First();
                SetPortalUserInSession(item);  
            }

            //string ee = _httpContextAccessor.Session.GetString("zzz") ?? "0";

            string portalUserClientID = _httpContextAccessor.Session.GetString("portalUserClientID") ?? "0";

            List<ClientType> Client = await GetClientById(portalUserClientID);

            if (Client.Any())
            {
                ClientType item = Client.First();
                SetClientInSession(item);
            }

            return new SessionInitializationResult { Success = true };
        }

        /// <summary>
        /// Gets the portal user by email.
        /// </summary>
        /// <param name="email">The email of the portal user.</param>
        /// <returns>A list of <see cref="PortalUserType"/> objects representing the portal user.</returns>
        private async Task<List<PortalUserType>> GetPortalUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            { // here
              throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }

            string sqlPortalUser = @"SELECT [portalUserID], [portalUserFullname], [portalUserFirstName], [portalUserSurname], [portalUserClientID], [portalUserEmail], [PortalUserClientsAdditionalIds],[portalUserCompany] FROM [dbo].[tblPortalUsers] WHERE [portalUserEmail] = @Email;";

            try
            {
                var emailParameter = new SqlParameter("Email", email);
                var queryResult = _context.QueryPortalUser.FromSqlRaw(sqlPortalUser, emailParameter).ToList();

                return queryResult.Select(u => new PortalUserType
                {
                    portalUserID = u.portalUserID,
                    portalUserFullname = u.portalUserFullname,
                    portalUserFirstName = u.portalUserFirstname,
                    portalUserSurname = u.portalUserSurname,
                    portalUserClientID = u.portalUserClientID,
                    portalUserEmail = u.portalUserEmail,
                    PortalUserClientsAdditionalIds = u.PortalUserClientsAdditionalIds,
                    portalUserCompany = u.portalUserCompany
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed.
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Sets the portal user in session.
        /// </summary>
        /// <param name="user">The portal user to set in session.</param>
        private void SetPortalUserInSession(PortalUserType user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            
            _httpContextAccessor.Session.SetString("portalUserID", user.portalUserID.ToString());
            _httpContextAccessor.Session.SetString("portalUserFullname", user.portalUserFullname);
            _httpContextAccessor.Session.SetString("portalUserFirstName", user.portalUserFirstName);
            _httpContextAccessor.Session.SetString("portalUserSurname", user.portalUserSurname);
            _httpContextAccessor.Session.SetString("portalUserClientID", user.portalUserClientID.ToString());
            _httpContextAccessor.Session.SetString("portalUserEmail", user.portalUserEmail);
            _httpContextAccessor.Session.SetString("portalUserCompany", user.portalUserCompany.ToString());

        }

        /// <summary>
        /// Gets the client by ID.
        /// </summary>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>A list of <see cref="ClientType"/> objects representing the client.</returns>
        private async Task<List<ClientType>> GetClientById(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));
            }

            string sqlClient = @"SELECT [clientID], [clientFirstname], [clientSurname], [clientAddress], [clientSuburb],
                         [clientState], [clientPostcode], [clientFullName] FROM [dbo].[tblClients] WHERE [clientID] = @ClientID;";

            var queryResult = _context.QueryClient.FromSqlRaw(sqlClient, new SqlParameter("@ClientID", clientId)).ToList();

            return queryResult.Select(c => new ClientType
            {
                clientID = c.clientID,
                clientFirstname = c.clientFirstname,
                clientSurname = c.clientSurname,
                clientAddress = c.clientAddress,
                clientSuburb = c.clientSuburb,
                clientState = c.clientState,
                clientPostcode = c.clientPostcode,
                clientFullName = c.clientFullName
            }).ToList();
        }

        /// <summary>
        /// Sets the client in session.
        /// </summary>
        /// <param name="client">The client to set in session.</param>
        private void SetClientInSession(ClientType client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "Client cannot be null.");
            }

            _httpContextAccessor.Session.SetString("clientFirstName", client.clientFirstname);
            _httpContextAccessor.Session.SetString("clientSurname", client.clientSurname);
            _httpContextAccessor.Session.SetString("clientFullName", client.clientFullName);
            string address = $"{client.clientAddress}, {client.clientSuburb}, {client.clientState}, {client.clientPostcode}";
            _httpContextAccessor.Session.SetString("clientAddress", address);
        }
    }

    /// <summary>
    /// Represents the result of a session initialization operation.
    /// </summary>
    public class SessionInitializationResult
    {
        /// <summary>
        /// Indicates if the session initialization was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Contains the error message if the session initialization failed.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
