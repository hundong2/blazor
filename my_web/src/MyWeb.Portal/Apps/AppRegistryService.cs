using Microsoft.EntityFrameworkCore;
using MyWeb.Portal.Data;

namespace MyWeb.Portal.Apps;

public sealed class AppRegistryService(
    ApplicationDbContext db,
    AppEndpointValidator endpointValidator,
    ProxyConfigManager proxyConfig)
{
    public Task<List<AppDefinition>> ListAsync(bool includeDisabled = true) =>
        db.AppDefinitions
            .Where(app => includeDisabled || app.Enabled)
            .OrderBy(app => app.SortOrder)
            .ThenBy(app => app.Name)
            .AsNoTracking()
            .ToListAsync();

    public Task<AppDefinition?> FindAsync(Guid id) =>
        db.AppDefinitions.AsNoTracking().SingleOrDefaultAsync(app => app.Id == id);

    public async Task<(bool Succeeded, string? Error)> CreateAsync(AppDefinitionInput input)
    {
        if (!endpointValidator.TryValidate(input.InternalUrl, out var endpoint, out var error))
        {
            return (false, error);
        }

        if (await db.AppDefinitions.AnyAsync(app => app.Slug == input.Slug))
        {
            return (false, "같은 URL 식별자가 이미 존재합니다.");
        }

        var entity = new AppDefinition();
        Apply(input, endpoint!, entity);
        db.AppDefinitions.Add(entity);
        await db.SaveChangesAsync();
        await ReloadProxyAsync();
        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> UpdateAsync(Guid id, AppDefinitionInput input)
    {
        if (!endpointValidator.TryValidate(input.InternalUrl, out var endpoint, out var error))
        {
            return (false, error);
        }

        var entity = await db.AppDefinitions.SingleOrDefaultAsync(app => app.Id == id);
        if (entity is null)
        {
            return (false, "서비스를 찾을 수 없습니다.");
        }

        if (await db.AppDefinitions.AnyAsync(app => app.Id != id && app.Slug == input.Slug))
        {
            return (false, "같은 URL 식별자가 이미 존재합니다.");
        }

        Apply(input, endpoint!, entity);
        await db.SaveChangesAsync();
        await ReloadProxyAsync();
        return (true, null);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await db.AppDefinitions.SingleOrDefaultAsync(app => app.Id == id);
        if (entity is null)
        {
            return false;
        }

        db.AppDefinitions.Remove(entity);
        await db.SaveChangesAsync();
        await ReloadProxyAsync();
        return true;
    }

    public async Task ReloadProxyAsync()
    {
        var applications = await db.AppDefinitions.AsNoTracking().ToListAsync();
        proxyConfig.Update(applications);
    }

    private static void Apply(AppDefinitionInput input, Uri endpoint, AppDefinition entity)
    {
        entity.Name = input.Name.Trim();
        entity.Slug = input.Slug.Trim().ToLowerInvariant();
        entity.InternalUrl = endpoint.AbsoluteUri;
        entity.Category = input.Category.Trim();
        entity.Icon = input.Icon.Trim();
        entity.HealthPath = input.HealthPath.Trim();
        entity.Enabled = input.Enabled;
        entity.SortOrder = input.SortOrder;
        entity.UpdatedAt = DateTimeOffset.UtcNow;
    }
}
