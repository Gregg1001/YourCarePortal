using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using YourCarePortal.Data;
using System;
using YourCarePortal.Models;
using Microsoft.Data.SqlClient;
using YourCarePortal.Services;

/// <summary>
/// Represents the main entry point of the application.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

// Retrieve the database connection string from the application's configuration.
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

// Validate the connection string to ensure it's not empty or null.
if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentNullException("connectionString", "Database connection string is null.");
}

/// <summary>
/// Service registrations and configurations.
/// </summary>

// Register MVC controllers and views along with Razor pages.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register and configure database context using the provided connection string.
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));

// Register a default HttpClient for making HTTP requests.
builder.Services.AddHttpClient();

// Configure various service options from the application's configuration.
builder.Services.Configure<ApiUrls>(builder.Configuration.GetSection("ApiUrls"));
builder.Services.Configure<OtherURLs>(builder.Configuration.GetSection("OtherUrls"));
builder.Services.Configure<NoDataMsg>(builder.Configuration.GetSection("NoDataMsg"));

// Register application-specific HttpClients for making HTTP requests.
builder.Services.AddHttpClient<AuthenticationHelper>();
builder.Services.AddHttpClient<ChangePasswordHelper>();
builder.Services.AddHttpClient<APIResponseHelper>();
builder.Services.AddHttpClient<ResponseHelper>();

// Register in-memory cache services.
builder.Services.AddMemoryCache();

// Reset Password Service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<PasswordResetService>();
builder.Services.AddScoped<PasswordResetActionService>();

// Custom Forms Service
builder.Services.AddScoped<ClientService>();

// Register session services and configure session properties.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register GetAuthKeyService
builder.Services.AddScoped<GetAuthKeyService>();

// Register UrlParameterService
builder.Services.AddScoped<UrlParameterService>();

// Register the for Client Details application's services.
builder.Services.AddScoped<ApiUrls>();
builder.Services.AddScoped<GetWhitelabelImageLocation>();
builder.Services.AddScoped<AuthenticationHelper>();
builder.Services.AddScoped<ClientDataHelperNew>();
builder.Services.AddScoped<SessionPreLoadDataService>();

// Register the for Client Details application's services.
builder.Services.AddHttpContextAccessor();

/// <summary>
/// Build the application instance.
/// </summary>
var app = builder.Build();

/// <summary>
/// Configure the HTTP request pipeline.
/// </summary>

// Middleware to set the application's culture info to US English.
app.Use(async (context, next) =>
{
    CultureInfo.CurrentCulture = new CultureInfo("en-US");
    CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    await next.Invoke();
});

// Configure error handling and HSTS based on the environment.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware to enforce HTTPS and serve static files.
app.UseHttpsRedirection();
app.UseStaticFiles();

// Middleware to configure routing and session management.
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Define default routing patterns.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Appointments}/{action=Appointments}/{id?}"
);

// Define the application's endpoint mappings.
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Appointments}/{action=Appointments}/{id?}"
    );

    // Redirect root requests to the Appointments page.
    endpoints.MapGet("/", context =>
    {
        context.Response.Redirect("/Appointments/Appointments");
        return Task.CompletedTask;
    });
});

// Run the application.
app.Run();
