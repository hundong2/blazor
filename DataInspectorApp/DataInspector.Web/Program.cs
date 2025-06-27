using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using DataInspector.SharedComponents; // Added to reference App from SharedComponents

namespace DataInspector.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Explicitly set the App component from DataInspector.SharedComponents as the root.
            // This assumes DataInspector.Web project might not have its own App.razor or it should be ignored.
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }
    }
}
