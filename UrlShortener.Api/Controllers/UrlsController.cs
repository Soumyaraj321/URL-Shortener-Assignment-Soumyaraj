using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UrlShortener.Api.DTOs;
using UrlShortener.Api.Services;
using UrlShortener.Api.Services.Validators;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/urls")]
public class UrlsController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;
    private readonly ShortCodeValidator _shortCodeValidator;

    public UrlsController(
        IUrlShortenerService urlShortenerService,
        ShortCodeValidator shortCodeValidator)
    {
        _urlShortenerService = urlShortenerService;
        _shortCodeValidator = shortCodeValidator;
    }

    [HttpPost]
    [EnableRateLimiting("url-creation")]
    [ProducesResponseType(
        typeof(CreateShortUrlResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateShortUrlRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _urlShortenerService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(Get),
            new { shortCode = result.ShortCode },
            result);
    }

    [HttpGet("{shortCode}")]
    [ProducesResponseType(
        typeof(ShortUrlResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        string shortCode,
        CancellationToken cancellationToken)
    {
        if (!_shortCodeValidator.IsValid(shortCode))
        {
            return BadRequest(new
            {
                error = "Invalid short code."
            });
        }

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

    [Authorize(Roles = "Admin")]
    [HttpGet("{shortCode}/analytics")]
    [ProducesResponseType(
        typeof(AnalyticsResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnalytics(
        string shortCode,
        CancellationToken cancellationToken)
    {
        if (!_shortCodeValidator.IsValid(shortCode))
        {
            return BadRequest(new
            {
                error = "Invalid short code."
            });
        }

        var result =
            await _urlShortenerService.GetAnalyticsAsync(
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

    [Authorize(Roles = "Admin")]
    [HttpDelete("{shortCode}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(
        string shortCode,
        CancellationToken cancellationToken)
    {
        if (!_shortCodeValidator.IsValid(shortCode))
        {
            return BadRequest(new
            {
                error = "Invalid short code."
            });
        }

        var deactivated =
            await _urlShortenerService.DeactivateAsync(
                shortCode,
                cancellationToken);

        if (!deactivated)
        {
            return NotFound(new
            {
                error = "Short URL not found."
            });
        }

        return NoContent();
    }
}