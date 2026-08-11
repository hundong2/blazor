using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MyWeb.Portal.Apps;
using MyWeb.Portal.Data;
using Yarp.ReverseProxy.Configuration;

namespace MyWeb.Portal.Tests;

public sealed class AppRegistryServiceTests
{
    [Fact]
    public async Task CreatePersistsApplicationAndReloadsProxy()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var db = CreateDbContext();
        await db.Database.OpenConnectionAsync(cancellationToken);
        await db.Database.EnsureCreatedAsync(cancellationToken);
        var provider = new InMemoryConfigProvider([], []);
        var proxy = new ProxyConfigManager(
            provider,
            new AppEndpointValidator(),
            NullLogger<ProxyConfigManager>.Instance);
        var registry = new AppRegistryService(db, new AppEndpointValidator(), proxy);
        var input = new AppDefinitionInput
        {
            Name = "Open WebUI",
            Slug = "open-webui",
            InternalUrl = "http://127.0.0.1:17832",
            Category = "AI",
            HealthPath = "/health"
        };

        var result = await registry.CreateAsync(input);

        Assert.True(result.Succeeded, result.Error);
        var saved = await db.AppDefinitions.SingleAsync(cancellationToken);
        Assert.Equal("http://127.0.0.1:17832/", saved.InternalUrl);
        Assert.Equal("/apps/open-webui/{**catch-all}", provider.GetConfig().Routes.Single().Match.Path);
    }

    [Fact]
    public async Task DuplicateSlugDoesNotReplaceExistingRoute()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var db = CreateDbContext();
        await db.Database.OpenConnectionAsync(cancellationToken);
        await db.Database.EnsureCreatedAsync(cancellationToken);
        var provider = new InMemoryConfigProvider([], []);
        var registry = new AppRegistryService(
            db,
            new AppEndpointValidator(),
            new ProxyConfigManager(
                provider,
                new AppEndpointValidator(),
                NullLogger<ProxyConfigManager>.Instance));
        var first = new AppDefinitionInput
        {
            Name = "First",
            Slug = "same",
            InternalUrl = "http://127.0.0.1:17832"
        };
        var duplicate = new AppDefinitionInput
        {
            Name = "Second",
            Slug = "same",
            InternalUrl = "http://127.0.0.1:17833"
        };

        Assert.True((await registry.CreateAsync(first)).Succeeded);
        var result = await registry.CreateAsync(duplicate);

        Assert.False(result.Succeeded);
        Assert.Single(await db.AppDefinitions.ToListAsync(cancellationToken));
        Assert.Single(provider.GetConfig().Routes);
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        return new ApplicationDbContext(options);
    }
}
