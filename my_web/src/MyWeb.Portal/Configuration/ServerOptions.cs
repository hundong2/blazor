using System.ComponentModel.DataAnnotations;
using System.Net;

namespace MyWeb.Portal.Configuration;

public sealed class ServerOptions : IValidatableObject
{
    public const string SectionName = "Server";

    [Required]
    public string BindAddress { get; init; } = "127.0.0.1";

    [Range(1024, 49151)]
    public int Port { get; init; } = 17831;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IPAddress.TryParse(BindAddress, out var address))
        {
            yield return new ValidationResult(
                "Server:BindAddress must be a valid IP address.",
                [nameof(BindAddress)]);
            yield break;
        }

        if (!IPAddress.IsLoopback(address))
        {
            yield return new ValidationResult(
                "Server:BindAddress must be a loopback address. IIS is the only public entry point.",
                [nameof(BindAddress)]);
        }

        int[] discouragedPorts = [3000, 5000, 5001, 8000, 8080, 11434];
        if (discouragedPorts.Contains(Port))
        {
            yield return new ValidationResult(
                "Choose a dedicated internal port instead of a common development/service port.",
                [nameof(Port)]);
        }
    }
}
