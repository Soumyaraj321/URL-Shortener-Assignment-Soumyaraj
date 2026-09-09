namespace UrlShortener.Api.Domain.Entities;

public class ShortUrl
{
    public Guid Id { get; set; }

    public string ShortCode { get; set; } = string.Empty;

    public string LongUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public int ClickCount { get; set; }

    public DateTime? LastAccessedAt { get; set; }

    public ICollection<Click> Clicks { get; set; } = new List<Click>();
}