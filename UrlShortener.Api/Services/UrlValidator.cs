using UrlShortener.Api.Services.Interfaces;

namespace UrlShortener.Api.Services;

public class UrlValidator : IUrlValidator
{
    private const int MaxUrlLength = 2048;

    public bool IsValid(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        url = url.Trim();

        if (url.Length > MaxUrlLength)
        {
            return false;
        }

        if (!Uri.TryCreate(
                url,
                UriKind.Absolute,
                out var uri))
        {
            return false;
        }

        if (!uri.Scheme.Equals(
                Uri.UriSchemeHttp,
                StringComparison.OrdinalIgnoreCase)
            && !uri.Scheme.Equals(
                Uri.UriSchemeHttps,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(uri.Host);
    }
}