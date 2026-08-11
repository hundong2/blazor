using System.ComponentModel.DataAnnotations;

namespace MyWeb.Portal.Configuration;

public sealed class MyWebOptions : IValidatableObject
{
    public const string SectionName = "MyWeb";

    [Required]
    public string PublicUrl { get; init; } = "https://portal.example.com";

    [Required]
    public string OwnerEmail { get; init; } = "owner@example.com";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Uri.TryCreate(PublicUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            yield return new ValidationResult(
                "MyWeb:PublicUrl must be an absolute HTTPS URL.",
                [nameof(PublicUrl)]);
        }
    }
}
