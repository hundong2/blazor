using Microsoft.SemanticKernel;
using System.Threading.Tasks;

namespace BlazorAppLee.Data
{
    public class ChatService
    {
        private readonly Kernel? _kernel;
        private readonly bool _isApiKeyConfigured;

        public ChatService(IConfiguration configuration)
        {
            var apiKey = configuration["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_OPENAI_API_KEY")
            {
                _isApiKeyConfigured = false;
                return;
            }

            _isApiKeyConfigured = true;
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(
                modelId: "gpt-3.5-turbo",
                apiKey: apiKey);
            builder.Plugins.AddFromType<ToolPlugins>();
            _kernel = builder.Build();
        }

        public async Task<string> ProcessUserMessageAsync(string message)
        {
            if (!_isApiKeyConfigured || _kernel is null)
            {
                return "Please configure your OpenAI API key in appsettings.json.";
            }

            var planner = new Microsoft.SemanticKernel.Planners.Handlebars.HandlebarsPlanner();
            var plan = await planner.CreatePlanAsync(_kernel, message);
            var result = await plan.InvokeAsync(_kernel);
            return result ?? "No response from AI.";
        }
    }
}
