using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorAppLee.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Required for ApplicationDbContext placeholder

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
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddAntiforgery(); // Add Antiforgery services

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
