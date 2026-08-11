using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyWeb.Portal.Apps;
using MyWeb.Portal.Configuration;
using MyWeb.Portal.Data;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

var programDataRoot = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
    "MyWeb");
var defaultProductionConfig = Path.Combine(programDataRoot, "config", "appsettings.Production.json");
var externalConfig = Environment.GetEnvironmentVariable("MYWEB_CONFIG_FILE");

builder.Configuration.AddJsonFile(defaultProductionConfig, optional: true, reloadOnChange: true);
if (!string.IsNullOrWhiteSpace(externalConfig))
{
    builder.Configuration.AddJsonFile(Path.GetFullPath(externalConfig), optional: false, reloadOnChange: true);
}

// Re-add these providers so machine-specific values always win over JSON files.
builder.Configuration.AddEnvironmentVariables(prefix: "MYWEB_");
builder.Configuration.AddCommandLine(args);

builder.Services
    .AddOptions<ServerOptions>()
    .Bind(builder.Configuration.GetSection(ServerOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services
    .AddOptions<MyWebOptions>()
    .Bind(builder.Configuration.GetSection(MyWebOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var serverOptions = builder.Configuration
    .GetRequiredSection(ServerOptions.SectionName)
    .Get<ServerOptions>() ?? throw new InvalidOperationException("Server configuration is missing.");

// Local development and out-of-process hosting use this listener. IIS remains the public HTTPS edge.
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Parse(serverOptions.BindAddress), serverOptions.Port);
    options.AddServerHeader = false;
});

var dataRoot = builder.Configuration["Storage:DataRoot"];
if (string.IsNullOrWhiteSpace(dataRoot))
{
    dataRoot = Path.Combine(programDataRoot, "data");
}

var databasePath = Path.Combine(dataRoot, "myweb.db");
var keyPath = Path.Combine(dataRoot, "keys");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));
var dataProtection = builder.Services
    .AddDataProtection()
    .SetApplicationName("MyWeb.Portal")
    .PersistKeysToFileSystem(new DirectoryInfo(keyPath));
if (OperatingSystem.IsWindows())
{
    dataProtection.ProtectKeysWithDpapi(protectToLocalMachine: true);
}

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 14;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = builder.Environment.IsDevelopment()
        ? "MyWeb.Auth.Development"
        : "__Host-MyWeb.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/denied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/health"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OwnerOnly", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("SensitiveAccess", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("amr", "mfa");
    });
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddRazorPages();
var proxyProvider = new InMemoryConfigProvider([], []);
builder.Services.AddSingleton(proxyProvider);
builder.Services.AddSingleton<IProxyConfigProvider>(proxyProvider);
builder.Services.AddSingleton<AppEndpointValidator>();
builder.Services.AddSingleton<ProxyConfigManager>();
builder.Services.AddScoped<AppRegistryService>();
builder.Services.AddReverseProxy();
builder.Services.AddHttpClient("AppHealth")
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        AllowAutoRedirect = false,
        ConnectTimeout = TimeSpan.FromSeconds(3)
    });
builder.Services.AddHostedService<AppHealthCheckWorker>();
builder.Services.AddHealthChecks();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto |
                               ForwardedHeaders.XForwardedHost;
    options.KnownProxies.Add(IPAddress.Loopback);
    options.KnownProxies.Add(IPAddress.IPv6Loopback);
});

var app = builder.Build();

// Force all startup validation before creating directories, databases, or listeners.
_ = app.Services.GetRequiredService<IOptions<ServerOptions>>().Value;
_ = app.Services.GetRequiredService<IOptions<MyWebOptions>>().Value;
Directory.CreateDirectory(dataRoot);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    var registry = scope.ServiceProvider.GetRequiredService<AppRegistryService>();
    await registry.ReloadProxyAsync();
}

if (builder.Configuration.GetValue<bool>("bootstrap-admin"))
{
    await BootstrapAdminAsync(app.Services, builder.Configuration["email"]);
    return;
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true &&
        !context.Request.Path.StartsWithSegments("/account/setup2fa") &&
        !context.Request.Path.StartsWithSegments("/account/logout") &&
        !context.Request.Path.StartsWithSegments("/health"))
    {
        var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.GetUserAsync(context.User);
        if (user is not null && !await userManager.GetTwoFactorEnabledAsync(user))
        {
            context.Response.Redirect("/account/setup2fa");
            return;
        }
    }

    await next();
});
app.UseAuthorization();

app.MapHealthChecks("/health/live").AllowAnonymous();
app.MapHealthChecks("/health/ready").RequireAuthorization("OwnerOnly");
app.MapRazorPages();
app.MapReverseProxy();

app.Run();

static async Task BootstrapAdminAsync(IServiceProvider services, string? requestedEmail)
{
    using var scope = services.CreateScope();
    var options = scope.ServiceProvider.GetRequiredService<IOptions<MyWebOptions>>().Value;
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var email = string.IsNullOrWhiteSpace(requestedEmail) ? options.OwnerEmail : requestedEmail.Trim();

    if (await userManager.Users.AnyAsync())
    {
        Console.Error.WriteLine("Bootstrap refused: an owner account already exists.");
        Environment.ExitCode = 2;
        return;
    }

    Console.Write($"Create owner account '{email}'. Password: ");
    var password = ReadSecret();
    Console.Write("Confirm password: ");
    var confirmation = ReadSecret();

    if (!string.Equals(password, confirmation, StringComparison.Ordinal))
    {
        Console.Error.WriteLine("Passwords do not match.");
        Environment.ExitCode = 3;
        return;
    }

    var user = new ApplicationUser
    {
        UserName = email,
        Email = email,
        EmailConfirmed = true
    };
    var result = await userManager.CreateAsync(user, password);
    if (!result.Succeeded)
    {
        foreach (var error in result.Errors)
        {
            Console.Error.WriteLine($"{error.Code}: {error.Description}");
        }

        Environment.ExitCode = 4;
        return;
    }

    Console.WriteLine("Owner account created. Public registration remains disabled.");
}

static string ReadSecret()
{
    if (Console.IsInputRedirected)
    {
        return Console.ReadLine() ?? string.Empty;
    }

    var characters = new List<char>();
    while (true)
    {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            return new string([.. characters]);
        }

        if (key.Key == ConsoleKey.Backspace && characters.Count > 0)
        {
            characters.RemoveAt(characters.Count - 1);
            continue;
        }

        if (!char.IsControl(key.KeyChar))
        {
            characters.Add(key.KeyChar);
        }
    }
}

public partial class Program;
