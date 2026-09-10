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

        if (url.Length > MaxUrlLength)
        {
            return false;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        return uri.Scheme.Equals(
                   Uri.UriSchemeHttp,
                   StringComparison.OrdinalIgnoreCase)
               || uri.Scheme.Equals(
                   Uri.UriSchemeHttps,
                   StringComparison.OrdinalIgnoreCase);
    }
}