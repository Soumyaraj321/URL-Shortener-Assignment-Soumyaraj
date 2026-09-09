namespace UrlShortener.Api.Domain.Entities;

public class Click
{
    public Guid Id { get; set; }

    public Guid ShortUrlId { get; set; }

    public DateTime AccessedAt { get; set; }

    public string? UserAgent { get; set; }

    public string? Referrer { get; set; }

    public ShortUrl ShortUrl { get; set; } = null!;
}