using Microsoft.Extensions.Logging.Abstractions;
using MyWeb.Portal.Apps;
using MyWeb.Portal.Data;
using Yarp.ReverseProxy.Configuration;

namespace MyWeb.Portal.Tests;

public sealed class ProxyConfigManagerTests
{
    [Fact]
    public void BuildsOnlyEnabledOwnerProtectedRoutes()
    {
        var provider = new InMemoryConfigProvider([], []);
        var manager = new ProxyConfigManager(
            provider,
            new AppEndpointValidator(),
            NullLogger<ProxyConfigManager>.Instance);
        var enabled = new AppDefinition
        {
            Name = "AI",
            Slug = "ai",
            InternalUrl = "http://127.0.0.1:17832/",
            Enabled = true
        };
        var disabled = new AppDefinition
        {
            Name = "Disabled",
            Slug = "disabled",
            InternalUrl = "http://127.0.0.1:17833/",
            Enabled = false
        };
        var unsafeEndpoint = new AppDefinition
        {
            Name = "Unsafe",
            Slug = "unsafe",
            InternalUrl = "https://example.com:17834/",
            Enabled = true
        };

        manager.Update([enabled, disabled, unsafeEndpoint]);

        var config = provider.GetConfig();
        var route = Assert.Single(config.Routes);
        var cluster = Assert.Single(config.Clusters);
        Assert.Equal("OwnerOnly", route.AuthorizationPolicy);
        Assert.Equal("/apps/ai/{**catch-all}", route.Match.Path);
        Assert.Equal("http://127.0.0.1:17832/", cluster.Destinations!["primary"].Address);
    }
}
