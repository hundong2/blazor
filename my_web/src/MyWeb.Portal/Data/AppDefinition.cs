using System.ComponentModel.DataAnnotations;

namespace MyWeb.Portal.Data;

public sealed class AppDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(500)]
    public string InternalUrl { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Category { get; set; } = "기타";

    [MaxLength(80)]
    public string Icon { get; set; } = "app";

    [MaxLength(200)]
    public string HealthPath { get; set; } = "/";

    public bool Enabled { get; set; } = true;

    public int SortOrder { get; set; }

    public bool? LastHealthy { get; set; }

    public DateTimeOffset? LastCheckedAt { get; set; }

    [MaxLength(500)]
    public string? LastHealthError { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string PublicPath => $"/apps/{Slug}/";
}
