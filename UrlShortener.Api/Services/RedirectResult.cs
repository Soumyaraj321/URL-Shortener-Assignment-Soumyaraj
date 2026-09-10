namespace UrlShortener.Api.Services;

public enum RedirectStatus
{
    NotFound,
    Inactive,
    Active
}

public class RedirectResult
{
    public RedirectStatus Status { get; init; }

    public string? OriginalUrl { get; init; }
}