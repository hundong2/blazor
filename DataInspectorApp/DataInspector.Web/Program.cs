using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using DataInspector.SharedComponents;
using DataInspector.SharedComponents.Services;

namespace DataInspector.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Explicitly set the App component from DataInspector.SharedComponents as the root.
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Add MudBlazor services
            builder.Services.AddMudServices();

            // Add Authentication services
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            await builder.Build().RunAsync();
        }
    }
}
