using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using UrlShortener.Api.DTOs;

namespace UrlShortener.Tests.Integration;

public class UrlShortenerApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UrlShortenerApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    [Fact]
    public async Task CreateUrl_ReturnsCreatedResponse()
    {
        var request = new CreateShortUrlRequest
        {
            LongUrl = "https://example.com/products"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/urls",
            request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<CreateShortUrlResponse>();

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.ShortCode));
        Assert.Equal(
            $"/{result.ShortCode}",
            result.ShortUrl);
    }

    [Fact]
    public async Task GetUrl_ReturnsCreatedUrl()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/urls",
            new CreateShortUrlRequest
            {
                LongUrl = "https://example.com"
            });

        var created =
            await createResponse.Content
                .ReadFromJsonAsync<CreateShortUrlResponse>();

        var response = await _client.GetAsync(
            $"/api/urls/{created!.ShortCode}");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<ShortUrlResponse>();

        Assert.NotNull(result);
        Assert.Equal(
            "https://example.com",
            result.LongUrl);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task Redirect_IncrementsAnalytics()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/urls",
            new CreateShortUrlRequest
            {
                LongUrl = "https://example.com"
            });

        var created =
            await createResponse.Content
                .ReadFromJsonAsync<CreateShortUrlResponse>();

        var redirectResponse = await _client.GetAsync(
            $"/{created!.ShortCode}");

        Assert.Equal(
            HttpStatusCode.Redirect,
            redirectResponse.StatusCode);

        Assert.Equal(
            "https://example.com/",
            redirectResponse.Headers.Location?.ToString());

        var analyticsResponse = await _client.GetAsync(
            $"/api/urls/{created.ShortCode}/analytics");

        Assert.Equal(
            HttpStatusCode.OK,
            analyticsResponse.StatusCode);

        var analytics =
            await analyticsResponse.Content
                .ReadFromJsonAsync<AnalyticsResponse>();

        Assert.NotNull(analytics);
        Assert.Equal(1, analytics.TotalClicks);
        Assert.NotNull(analytics.LastAccessedAt);
    }

    [Fact]
    public async Task DeactivateUrl_PreventsRedirect()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/urls",
            new CreateShortUrlRequest
            {
                LongUrl = "https://example.com"
            });

        var created =
            await createResponse.Content
                .ReadFromJsonAsync<CreateShortUrlResponse>();

        var deleteResponse = await _client.DeleteAsync(
            $"/api/urls/{created!.ShortCode}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        var redirectResponse = await _client.GetAsync(
            $"/{created.ShortCode}");

        Assert.Equal(
            HttpStatusCode.Gone,
            redirectResponse.StatusCode);
    }

    [Fact]
    public async Task GetUnknownUrl_ReturnsNotFound()
    {
        var response = await _client.GetAsync(
            "/api/urls/unknown");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateInvalidUrl_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/urls",
            new CreateShortUrlRequest
            {
                LongUrl = "javascript:alert('xss')"
            });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}