using MyWeb.Portal.Data;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;

namespace MyWeb.Portal.Apps;

public sealed class ProxyConfigManager(
    InMemoryConfigProvider provider,
    AppEndpointValidator endpointValidator,
    ILogger<ProxyConfigManager> logger)
{
    public void Update(IReadOnlyCollection<AppDefinition> applications)
    {
        var enabled = applications
            .Where(app => app.Enabled)
            .Where(app => IsSafe(app))
            .ToArray();
        var routes = enabled.Select(CreateRoute).ToArray();
        var clusters = enabled.Select(CreateCluster).ToArray();

        provider.Update(routes, clusters);
        logger.LogInformation("Applied {Count} protected proxy routes.", routes.Length);
    }

    private bool IsSafe(AppDefinition app)
    {
        if (endpointValidator.TryValidate(app.InternalUrl, out _, out _))
        {
            return true;
        }

        logger.LogWarning("Skipped unsafe proxy endpoint for application {ApplicationId}.", app.Id);
        return false;
    }

    private static RouteConfig CreateRoute(AppDefinition app) => new()
    {
        RouteId = $"app-{app.Id:N}",
        ClusterId = $"app-{app.Id:N}",
        AuthorizationPolicy = "OwnerOnly",
        Match = new RouteMatch
        {
            Path = $"/apps/{app.Slug}/{{**catch-all}}"
        },
        Transforms =
        [
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["PathRemovePrefix"] = $"/apps/{app.Slug}"
            }
        ]
    };

    private static ClusterConfig CreateCluster(AppDefinition app) => new()
    {
        ClusterId = $"app-{app.Id:N}",
        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            ["primary"] = new() { Address = app.InternalUrl }
        },
        HttpRequest = new ForwarderRequestConfig
        {
            ActivityTimeout = TimeSpan.FromMinutes(5)
        }
    };
}
