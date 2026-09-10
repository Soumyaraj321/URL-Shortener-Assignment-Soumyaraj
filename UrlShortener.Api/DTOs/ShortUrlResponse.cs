namespace UrlShortener.Api.DTOs;

public class ShortUrlResponse
{
    public string ShortCode { get; set; } = string.Empty;

    public string LongUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}