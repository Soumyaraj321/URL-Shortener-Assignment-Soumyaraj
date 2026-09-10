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

        var result = await _urlShortenerService.GetRedirectUrlAsync(
            shortCode,
            userAgent,
            referrer,
            cancellationToken);

        if (result.Status == RedirectStatus.NotFound)
        {
            return NotFound(new
            {
                error = "Short URL not found."
            });
        }

        if (result.Status == RedirectStatus.Inactive)
        {
            return StatusCode(
                StatusCodes.Status410Gone,
                new
                {
                    error = "Short URL is no longer active."
                });
        }

        return Redirect(result.OriginalUrl!);
    }
}