using System.Security.Cryptography;

namespace UrlShortener.Api.Services;

public class ShortCodeGenerator : IShortCodeGenerator
{
    private const string Characters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string Generate(int length = 6)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length),
                "Length must be greater than zero.");
        }

        var result = new char[length];

        for (var i = 0; i < length; i++)
        {
            var index = RandomNumberGenerator.GetInt32(Characters.Length);
            result[i] = Characters[index];
        }

        return new string(result);
    }
}