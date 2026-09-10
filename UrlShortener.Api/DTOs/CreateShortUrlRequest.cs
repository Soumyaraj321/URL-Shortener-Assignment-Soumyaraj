using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Api.DTOs;

public class CreateShortUrlRequest
{
    [Required]
    [MaxLength(2048)]
    public string LongUrl { get; set; } = string.Empty;
}