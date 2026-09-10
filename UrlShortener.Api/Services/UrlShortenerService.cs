using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;
using UrlShortener.Api.Domain.Entities;
using UrlShortener.Api.DTOs;

namespace UrlShortener.Api.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private const int ShortCodeLength = 6;
    private const int MaxCollisionAttempts = 5;

    private readonly AppDbContext _dbContext;
    private readonly IUrlValidator _urlValidator;
    private readonly IShortCodeGenerator _shortCodeGenerator;

    public UrlShortenerService(
        AppDbContext dbContext,
        IUrlValidator urlValidator,
        IShortCodeGenerator shortCodeGenerator)
    {
        _dbContext = dbContext;
        _urlValidator = urlValidator;
        _shortCodeGenerator = shortCodeGenerator;
    }

    public async Task<CreateShortUrlResponse> CreateAsync(
        CreateShortUrlRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!_urlValidator.IsValid(request.LongUrl))
        {
            throw new ArgumentException(
                "The supplied URL is invalid.",
                nameof(request.LongUrl));
        }

        for (var attempt = 0; attempt < MaxCollisionAttempts; attempt++)
        {
            var shortCode = _shortCodeGenerator.Generate(ShortCodeLength);

            var exists = await _dbContext.ShortUrls
                .AnyAsync(
                    x => x.ShortCode == shortCode,
                    cancellationToken);

            if (exists)
            {
                continue;
            }

            var shortUrl = new ShortUrl
            {
                Id = Guid.NewGuid(),
                ShortCode = shortCode,
                LongUrl = request.LongUrl,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                ClickCount = 0
            };

            _dbContext.ShortUrls.Add(shortUrl);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreateShortUrlResponse
            {
                ShortCode = shortCode,
                ShortUrl = $"/{shortCode}"
            };
        }

        throw new InvalidOperationException(
            "Unable to generate a unique short code.");
    }

    public async Task<string?> GetRedirectUrlAsync(
        string shortCode,
        string? userAgent,
        string? referrer,
        CancellationToken cancellationToken = default)
    {
        var shortUrl = await _dbContext.ShortUrls
            .FirstOrDefaultAsync(
                x => x.ShortCode == shortCode,
                cancellationToken);

        if (shortUrl is null || !shortUrl.IsActive)
        {
            return null;
        }

        var click = new Click
        {
            Id = Guid.NewGuid(),
            ShortUrlId = shortUrl.Id,
            AccessedAt = DateTime.UtcNow,
            UserAgent = userAgent,
            Referrer = referrer
        };

        shortUrl.ClickCount++;
        shortUrl.LastAccessedAt = click.AccessedAt;

        _dbContext.Clicks.Add(click);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return shortUrl.LongUrl;
    }

    public async Task<ShortUrlResponse?> GetAsync(
string shortCode,
CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShortUrls
            .AsNoTracking()
            .Where(x => x.ShortCode == shortCode)
            .Select(x => new ShortUrlResponse
            {
                ShortCode = x.ShortCode,
                LongUrl = x.LongUrl,
                CreatedAt = x.CreatedAt,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AnalyticsResponse?> GetAnalyticsAsync(
    string shortCode,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShortUrls
            .AsNoTracking()
            .Where(x => x.ShortCode == shortCode)
            .Select(x => new AnalyticsResponse
            {
                ShortCode = x.ShortCode,
                LongUrl = x.LongUrl,
                TotalClicks = x.ClickCount,
                CreatedAt = x.CreatedAt,
                LastAccessedAt = x.LastAccessedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

}