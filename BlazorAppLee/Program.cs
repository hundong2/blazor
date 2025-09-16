using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorAppLee.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Required for ApplicationDbContext placeholder
using BlazorBootstrap; // Add BlazorBootstrap

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Placeholder for ApplicationDbContext - will be defined in the next step
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // Disable email confirmation
})
    .AddRoles<ApplicationRole>() // Add role support
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

// Blazor Server 설정 개선
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = builder.Environment.IsDevelopment();
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
    options.DisconnectedCircuitMaxRetained = 100;
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(1);
});

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddAntiforgery(); // Add Antiforgery services

// Add BlazorBootstrap services
builder.Services.AddBlazorBootstrap();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure Authentication middleware is added
app.UseAuthorization(); // Ensure Authorization middleware is added
app.UseAntiforgery(); // Add Antiforgery middleware

// Blazor Hub 매핑 개선
app.MapBlazorHub(options =>
{
    options.CloseOnAuthenticationExpiration = true;
});
app.MapFallbackToPage("/_Host");

app.Run();
