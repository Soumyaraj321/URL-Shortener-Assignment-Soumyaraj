using UrlShortener.Api.DTOs;

namespace UrlShortener.Api.Services.Interfaces;

public interface IUrlShortenerService
{
    Task<CreateShortUrlResponse> CreateAsync(
        CreateShortUrlRequest request,
        CancellationToken cancellationToken = default);

    Task<RedirectResult> GetRedirectUrlAsync(
        string shortCode,
        string? userAgent,
        string? referrer,
        CancellationToken cancellationToken = default);

    Task<ShortUrlResponse?> GetAsync(
        string shortCode,
        CancellationToken cancellationToken = default);

    Task<AnalyticsResponse?> GetAnalyticsAsync(
        string shortCode,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateAsync(
        string shortCode,
        CancellationToken cancellationToken = default);
}