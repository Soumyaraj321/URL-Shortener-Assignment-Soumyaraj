namespace UrlShortener.Tests.Integration;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;

    public int ExpiresInMinutes { get; set; }

    public string Role { get; set; } = string.Empty;
}