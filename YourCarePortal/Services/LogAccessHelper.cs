using Microsoft.AspNetCore.Http;
using System;
using YourCarePortal.Interfaces;

/// <summary>
/// Handles logging of page accesses.
/// </summary>
public class LogAccessHelper
{
    private readonly ILogAccess _logger;
    private readonly HttpContext _httpContext;

    public LogAccessHelper(ILogAccess logger, HttpContext httpContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
    }

    /// <summary>
    /// Logs the access details for a given page.
    /// </summary>
    /// <param name="pageName">Name of the accessed page.</param>
    public void LogPageAccess(string pageName)
    {
        DateTime currentDateTime = DateTime.Now;

        var portalUserID = _httpContext.Session.GetString("portalUserID") ?? string.Empty;
        var portalUserCompany = _httpContext.Session.GetString("portalUserCompany") ?? string.Empty;

        if(portalUserID == string.Empty || portalUserCompany == string.Empty)
        {
            throw new InvalidOperationException("Session variables portalUserID is empty.");
        }

        if (portalUserCompany == string.Empty)
        {
            throw new InvalidOperationException("Session variable portalUserCompany is empty.");
        }


        if (!int.TryParse(portalUserID, out int portalUserIDInt) || !int.TryParse(portalUserCompany, out int providerIDInt))
        {
            throw new InvalidOperationException("Failed to convert session variables to integers.");
        }

        _logger.LogAccess(pageName, portalUserIDInt, currentDateTime, providerIDInt);
    }
}
