using System.Net;

namespace MyWeb.Portal.Apps;

public sealed class AppEndpointValidator
{
    private static readonly HashSet<int> DiscouragedPorts =
    [
        3000, 5000, 5001, 8000, 8080, 11434
    ];

    public bool TryValidate(string value, out Uri? normalizedUri, out string? error)
    {
        normalizedUri = null;
        error = null;

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            error = "내부 URL은 http 또는 https 절대 URL이어야 합니다.";
            return false;
        }

        if (!string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
        {
            error = "내부 URL에 인증정보, query 또는 fragment를 넣을 수 없습니다.";
            return false;
        }

        if (!IPAddress.TryParse(uri.Host, out var address) || !IPAddress.IsLoopback(address))
        {
            error = "현재 MVP에서는 127.0.0.1 또는 ::1의 literal loopback 주소만 허용합니다.";
            return false;
        }

        if (uri.Port is < 1024 or > 49151 || DiscouragedPorts.Contains(uri.Port))
        {
            error = "내부 포트는 1024..49151 범위의 전용 포트를 사용해야 합니다.";
            return false;
        }

        var builder = new UriBuilder(uri)
        {
            Path = uri.AbsolutePath.EndsWith('/') ? uri.AbsolutePath : $"{uri.AbsolutePath}/",
            Query = string.Empty,
            Fragment = string.Empty
        };
        normalizedUri = builder.Uri;
        return true;
    }
}
