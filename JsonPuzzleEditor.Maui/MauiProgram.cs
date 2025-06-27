using Microsoft.Extensions.Logging;
// using JsonPuzzleEditor.SharedComponents; // Might be needed if referring to types directly here

namespace JsonPuzzleEditor.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    // fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold"); // Keep if needed
                });

            builder.Services.AddMauiBlazorWebView();

            // Enable Blazor Web Developer Tools in debug mode
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug(); // Keep default debug logging
#endif
            // Note: RootComponent for BlazorWebView is typically specified in the XAML
            // or .razor file where BlazorWebView is used, not globally here unless
            // you are replacing the entire MAUI app content with a single Blazor component
            // from the start, which is less common for hybrid apps but possible.
            // For this setup, we'll define the root component in MainPage.xaml.

            // Register the JsonSerializerService
            builder.Services.AddSingleton<JsonPuzzleEditor.SharedComponents.Services.JsonSerializerService>();

            return builder.Build();
        }
    }
}
