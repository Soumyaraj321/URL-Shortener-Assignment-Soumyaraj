using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Services;

namespace UrlShortener.Api.Controllers;

[ApiController]
public class RedirectController : ControllerBase
{
    private readonly IUrlShortenerService _urlShortenerService;

    public RedirectController(IUrlShortenerService urlShortenerService)
    {
        _urlShortenerService = urlShortenerService;
    }

    [HttpGet("/{shortCode}")]
    public async Task<IActionResult> RedirectToOriginalUrl(
        string shortCode,
        CancellationToken cancellationToken)
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        var referrer = Request.Headers.Referer.ToString();

        var originalUrl = await _urlShortenerService.GetRedirectUrlAsync(
            shortCode,
            userAgent,
            referrer,
            cancellationToken);

        if (originalUrl is null)
        {
            return NotFound(new
            {
                error = "Short URL not found or inactive."
            });
        }

        return Redirect(originalUrl);
    }
}