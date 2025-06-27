using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using JsonPuzzleEditor.Web; // Assuming this is the namespace for the Wasm project itself if needed for App component.
using JsonPuzzleEditor.SharedComponents.Services; // For JsonSerializerService

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Default Blazor Wasm setup might have an App component in the Wasm project.
// If JsonPuzzleEditor.SharedComponents provides a common App or MainLayout,
// you might use: builder.RootComponents.Add<JsonPuzzleEditor.SharedComponents.App>("#app");
// or builder.RootComponents.Add<JsonPuzzleEditor.SharedComponents.MainLayout>("#app");
// For now, we assume the Wasm project's App.razor is used, which in turn uses Pages/Index.razor,
// and Index.razor hosts the JsonEditor.

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register the JsonSerializerService
builder.Services.AddSingleton<JsonSerializerService>();

await builder.Build().RunAsync();
