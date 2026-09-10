using Microsoft.EntityFrameworkCore;
using Moq;
using UrlShortener.Api.Data;
using UrlShortener.Api.DTOs;
using UrlShortener.Api.Services;

namespace UrlShortener.Tests.Services;

public class UrlShortenerServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_WithValidUrl_CreatesShortUrl()
    {
        await using var dbContext = CreateDbContext();

        var validator = new Mock<IUrlValidator>();

        validator
            .Setup(x => x.IsValid("https://example.com"))
            .Returns(true);

        var generator = new Mock<IShortCodeGenerator>();

        generator
            .Setup(x => x.Generate(6))
            .Returns("abc123");

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var request = new CreateShortUrlRequest
        {
            LongUrl = "https://example.com"
        };

        var result = await service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("abc123", result.ShortCode);
        Assert.Equal("/abc123", result.ShortUrl);

        var savedUrl = await dbContext.ShortUrls
            .FirstOrDefaultAsync(x => x.ShortCode == "abc123");

        Assert.NotNull(savedUrl);
        Assert.Equal(
            "https://example.com",
            savedUrl.LongUrl);

        Assert.True(savedUrl.IsActive);
        Assert.Equal(0, savedUrl.ClickCount);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidUrl_ThrowsArgumentException()
    {
        await using var dbContext = CreateDbContext();

        var validator = new Mock<IUrlValidator>();

        validator
            .Setup(x => x.IsValid("invalid-url"))
            .Returns(false);

        var generator = new Mock<IShortCodeGenerator>();

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var request = new CreateShortUrlRequest
        {
            LongUrl = "invalid-url"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(request));

        generator.Verify(
            x => x.Generate(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenCollisionOccurs_RetriesGeneration()
    {
        await using var dbContext = CreateDbContext();

        dbContext.ShortUrls.Add(new UrlShortener.Api.Domain.Entities.ShortUrl
        {
            Id = Guid.NewGuid(),
            ShortCode = "abc123",
            LongUrl = "https://existing.com",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            ClickCount = 0
        });

        await dbContext.SaveChangesAsync();

        var validator = new Mock<IUrlValidator>();

        validator
            .Setup(x => x.IsValid("https://example.com"))
            .Returns(true);

        var generator = new Mock<IShortCodeGenerator>();

        generator
            .SetupSequence(x => x.Generate(6))
            .Returns("abc123")
            .Returns("xyz789");

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var request = new CreateShortUrlRequest
        {
            LongUrl = "https://example.com"
        };

        var result = await service.CreateAsync(request);

        Assert.Equal("xyz789", result.ShortCode);

        generator.Verify(
            x => x.Generate(6),
            Times.Exactly(2));
    }

    [Fact]
    public async Task GetAsync_WithExistingShortCode_ReturnsUrl()
    {
        await using var dbContext = CreateDbContext();

        dbContext.ShortUrls.Add(
            new UrlShortener.Api.Domain.Entities.ShortUrl
            {
                Id = Guid.NewGuid(),
                ShortCode = "abc123",
                LongUrl = "https://example.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                ClickCount = 0
            });

        await dbContext.SaveChangesAsync();

        var validator = new Mock<IUrlValidator>();
        var generator = new Mock<IShortCodeGenerator>();

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var result = await service.GetAsync("abc123");

        Assert.NotNull(result);
        Assert.Equal("abc123", result.ShortCode);
        Assert.Equal(
            "https://example.com",
            result.LongUrl);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetAsync_WithUnknownShortCode_ReturnsNull()
    {
        await using var dbContext = CreateDbContext();

        var validator = new Mock<IUrlValidator>();
        var generator = new Mock<IShortCodeGenerator>();

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var result = await service.GetAsync("unknown");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetRedirectUrlAsync_WithActiveUrl_ReturnsOriginalUrl()
    {
        await using var dbContext = CreateDbContext();

        var shortUrl =
            new UrlShortener.Api.Domain.Entities.ShortUrl
            {
                Id = Guid.NewGuid(),
                ShortCode = "abc123",
                LongUrl = "https://example.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                ClickCount = 0
            };

        dbContext.ShortUrls.Add(shortUrl);

        await dbContext.SaveChangesAsync();

        var validator = new Mock<IUrlValidator>();
        var generator = new Mock<IShortCodeGenerator>();

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var result =
            await service.GetRedirectUrlAsync(
                "abc123",
                "Test-Agent",
                "https://google.com");

        Assert.Equal(
            RedirectStatus.Active,
            result.Status);

        Assert.Equal(
            "https://example.com",
            result.OriginalUrl);

        var updated =
            await dbContext.ShortUrls
                .FirstAsync(x => x.ShortCode == "abc123");

        Assert.Equal(1, updated.ClickCount);
        Assert.NotNull(updated.LastAccessedAt);

        var click =
            await dbContext.Clicks
                .FirstOrDefaultAsync();

        Assert.NotNull(click);
        Assert.Equal("Test-Agent", click.UserAgent);
        Assert.Equal(
            "https://google.com",
            click.Referrer);
    }

    [Fact]
    public async Task GetRedirectUrlAsync_WithUnknownUrl_ReturnsNotFound()
    {
        await using var dbContext = CreateDbContext();

        var validator = new Mock<IUrlValidator>();
        var generator = new Mock<IShortCodeGenerator>();

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var result =
            await service.GetRedirectUrlAsync(
                "unknown",
                null,
                null);

        Assert.Equal(
            RedirectStatus.NotFound,
            result.Status);
    }

    [Fact]
    public async Task GetRedirectUrlAsync_WithInactiveUrl_ReturnsInactive()
    {
        await using var dbContext = CreateDbContext();

        dbContext.ShortUrls.Add(
            new UrlShortener.Api.Domain.Entities.ShortUrl
            {
                Id = Guid.NewGuid(),
                ShortCode = "abc123",
                LongUrl = "https://example.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
                ClickCount = 5
            });

        await dbContext.SaveChangesAsync();

        var validator = new Mock<IUrlValidator>();
        var generator = new Mock<IShortCodeGenerator>();

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var result =
            await service.GetRedirectUrlAsync(
                "abc123",
                null,
                null);

        Assert.Equal(
            RedirectStatus.Inactive,
            result.Status);
    }

    [Fact]
    public async Task GetAnalyticsAsync_ReturnsAnalytics()
    {
        await using var dbContext = CreateDbContext();

        var createdAt = DateTime.UtcNow.AddHours(-2);
        var lastAccessedAt = DateTime.UtcNow;

        dbContext.ShortUrls.Add(
            new UrlShortener.Api.Domain.Entities.ShortUrl
            {
                Id = Guid.NewGuid(),
                ShortCode = "abc123",
                LongUrl = "https://example.com",
                CreatedAt = createdAt,
                IsActive = true,
                ClickCount = 10,
                LastAccessedAt = lastAccessedAt
            });

        await dbContext.SaveChangesAsync();

        var validator = new Mock<IUrlValidator>();
        var generator = new Mock<IShortCodeGenerator>();

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var result =
            await service.GetAnalyticsAsync("abc123");

        Assert.NotNull(result);
        Assert.Equal("abc123", result.ShortCode);
        Assert.Equal(10, result.TotalClicks);
        Assert.Equal(
            "https://example.com",
            result.LongUrl);
        Assert.Equal(
            lastAccessedAt,
            result.LastAccessedAt);
    }

    [Fact]
    public async Task GetAnalyticsAsync_WithUnknownUrl_ReturnsNull()
    {
        await using var dbContext = CreateDbContext();

        var validator = new Mock<IUrlValidator>();
        var generator = new Mock<IShortCodeGenerator>();

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var result =
            await service.GetAnalyticsAsync("unknown");

        Assert.Null(result);
    }

    [Fact]
    public async Task DeactivateAsync_WithExistingUrl_ReturnsTrue()
    {
        await using var dbContext = CreateDbContext();

        dbContext.ShortUrls.Add(
            new UrlShortener.Api.Domain.Entities.ShortUrl
            {
                Id = Guid.NewGuid(),
                ShortCode = "abc123",
                LongUrl = "https://example.com",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                ClickCount = 3
            });

        await dbContext.SaveChangesAsync();

        var validator = new Mock<IUrlValidator>();
        var generator = new Mock<IShortCodeGenerator>();

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var result =
            await service.DeactivateAsync("abc123");

        Assert.True(result);

        var updated =
            await dbContext.ShortUrls
                .FirstAsync(x => x.ShortCode == "abc123");

        Assert.False(updated.IsActive);
    }

    [Fact]
    public async Task DeactivateAsync_WithUnknownUrl_ReturnsFalse()
    {
        await using var dbContext = CreateDbContext();

        var validator = new Mock<IUrlValidator>();
        var generator = new Mock<IShortCodeGenerator>();

        var service = new UrlShortenerService(
            dbContext,
            validator.Object,
            generator.Object);

        var result =
            await service.DeactivateAsync("unknown");

        Assert.False(result);
    }
}