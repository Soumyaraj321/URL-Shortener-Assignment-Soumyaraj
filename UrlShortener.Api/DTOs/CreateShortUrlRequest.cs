namespace UrlShortener.Api.DTOs;

public class CreateShortUrlRequest
{
    public string LongUrl { get; set; } = string.Empty;
}