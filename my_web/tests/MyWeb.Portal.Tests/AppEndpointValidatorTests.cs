using MyWeb.Portal.Apps;

namespace MyWeb.Portal.Tests;

public sealed class AppEndpointValidatorTests
{
    private readonly AppEndpointValidator validator = new();

    [Theory]
    [InlineData("http://127.0.0.1:17832", "http://127.0.0.1:17832/")]
    [InlineData("http://[::1]:17833/base", "http://[::1]:17833/base/")]
    public void AcceptsDedicatedLoopbackEndpoints(string value, string expected)
    {
        var succeeded = validator.TryValidate(value, out var endpoint, out var error);

        Assert.True(succeeded, error);
        Assert.Equal(expected, endpoint!.AbsoluteUri);
    }

    [Theory]
    [InlineData("http://0.0.0.0:17832")]
    [InlineData("http://192.168.0.10:17832")]
    [InlineData("https://example.com:17832")]
    [InlineData("http://localhost:17832")]
    public void RejectsNonLiteralOrNonLoopbackHosts(string value)
    {
        Assert.False(validator.TryValidate(value, out _, out var error));
        Assert.Contains("loopback", error, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("http://127.0.0.1:80")]
    [InlineData("http://127.0.0.1:8080")]
    [InlineData("http://127.0.0.1:11434")]
    [InlineData("http://127.0.0.1:60000")]
    public void RejectsPrivilegedCommonOrEphemeralPorts(string value)
    {
        Assert.False(validator.TryValidate(value, out _, out var error));
        Assert.Contains("포트", error, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("ftp://127.0.0.1:17832")]
    [InlineData("not-a-url")]
    [InlineData("http://user:password@127.0.0.1:17832")]
    [InlineData("http://127.0.0.1:17832/?token=secret")]
    public void RejectsUnsupportedOrCredentialBearingUrls(string value)
    {
        Assert.False(validator.TryValidate(value, out _, out _));
    }
}
