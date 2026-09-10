namespace UrlShortener.Api.Services.Validators;

public class ShortCodeValidator
{
    private const int MaxLength = 10;

    public bool IsValid(string shortCode)
    {
        if (string.IsNullOrWhiteSpace(shortCode))
        {
            return false;
        }

        if (shortCode.Length > MaxLength)
        {
            return false;
        }

        return shortCode.All(char.IsLetterOrDigit);
    }
}