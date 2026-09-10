namespace UrlShortener.Api.DTOs;

public class AnalyticsResponse
{
    public string ShortCode { get; set; } = string.Empty;

    public string LongUrl { get; set; } = string.Empty;

    public int TotalClicks { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastAccessedAt { get; set; }
}