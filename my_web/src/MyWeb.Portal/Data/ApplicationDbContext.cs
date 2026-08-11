using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyWeb.Portal.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<AppDefinition> AppDefinitions => Set<AppDefinition>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppDefinition>(entity =>
        {
            entity.HasIndex(app => app.Slug).IsUnique();
            entity.Property(app => app.Name).IsRequired();
            entity.Property(app => app.Slug).IsRequired();
            entity.Property(app => app.InternalUrl).IsRequired();
        });
    }
}
