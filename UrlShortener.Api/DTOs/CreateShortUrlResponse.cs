namespace UrlShortener.Api.DTOs;

public class CreateShortUrlResponse
{
    public string ShortCode { get; set; } = string.Empty;

    public string ShortUrl { get; set; } = string.Empty;
}