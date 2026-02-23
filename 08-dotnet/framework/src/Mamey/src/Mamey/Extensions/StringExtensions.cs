using System.Text.RegularExpressions;

namespace Mamey;

public static class StringExtensions
{
    public static string RemoveWhitespace(this string value)
    => string.IsNullOrWhiteSpace(value) ? value : Regex.Replace(value, @"\s+", string.Empty);

    public static string ToCamelCase(this string str)
    {
        var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);
        var leadWord = Regex.Replace(words[0], @"([A-Z])([A-Z]+|[a-z0-9]+)($|[A-Z]\w*)",
            m =>
            {
                return m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value;
            });
        var tailWords = words.Skip(1)
            .Select(word => char.ToUpper(word[0]) + word.Substring(1))
            .ToArray();
        return $"{leadWord}{string.Join(string.Empty, tailWords)}";
    }
    public static string Underscore(this string value)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
                .ToLowerInvariant();

    public static string ToSnakeCase(this string value)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
                .ToLowerInvariant();

    public static string ToKebabCase(this string value)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x : x.ToString()))
                .ToLowerInvariant();

    public static int GenerateLuhnCheckDigit(this string input)
    {
        int sum = 0;
        bool alternate = false;

        // Process each digit from right to left
        for (int i = input.Length - 1; i >= 0; i--)
        {
            int n;
            if (Char.IsDigit(input[i]))
            {
                n = int.Parse(input[i].ToString());
            }
            else
            {
                n = input[i];
            }

            if (alternate)
            {
                n *= 2;
                if (n > 9)
                {
                    n -= 9;
                }
            }
            sum += n;
            alternate = !alternate;
        }

        // Luhn algorithm: The check digit is the amount that you must add to get the next multiple of 10
        int checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit;
    }
}

public static class Base64UrlExtensions
{
    public static string Base64UrlEncode(this byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}
