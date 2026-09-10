using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.DTOs;
using UrlShortener.Api.Services;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/urls")]
public class UrlsController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;

    public UrlsController(IUrlShortenerService urlShortenerService)
    {
        _urlShortenerService = urlShortenerService;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CreateShortUrlResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateShortUrlRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _urlShortenerService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new { shortCode = result.ShortCode },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                error = ex.Message
            });
        }
    }

    [HttpGet("{shortCode}")]
    [ProducesResponseType(
        typeof(ShortUrlResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        string shortCode,
        CancellationToken cancellationToken)
    {
        var result = await _urlShortenerService.GetAsync(
            shortCode,
            cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                error = "Short URL not found."
            });
        }

        return Ok(result);
    }

    [HttpGet("{shortCode}/analytics")]
    [ProducesResponseType(
        typeof(AnalyticsResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnalytics(
        string shortCode,
        CancellationToken cancellationToken)
    {
        var result = await _urlShortenerService.GetAnalyticsAsync(
            shortCode,
            cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                error = "Short URL not found."
            });
        }

        return Ok(result);
    }
}