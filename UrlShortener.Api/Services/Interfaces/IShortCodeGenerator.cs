namespace UrlShortener.Api.Services.Interfaces;

public interface IShortCodeGenerator
{
    string Generate(int length = 6);
}