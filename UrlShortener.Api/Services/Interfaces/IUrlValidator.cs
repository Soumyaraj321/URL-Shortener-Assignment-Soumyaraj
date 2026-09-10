namespace UrlShortener.Api.Services.Interfaces;

public interface IUrlValidator
{
    bool IsValid(string url);
}