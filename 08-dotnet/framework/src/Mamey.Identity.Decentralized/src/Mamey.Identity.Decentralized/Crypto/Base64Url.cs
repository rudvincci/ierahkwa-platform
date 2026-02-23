namespace Mamey.Identity.Decentralized.Crypto;

/// <summary>
/// Utility for Base64Url encoding/decoding per RFC 7515.
/// </summary>
public static class Base64Url
{
    public static string Encode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static byte[] Decode(string input)
    {
        string s = input.Replace('-', '+').Replace('_', '/');
        switch (input.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }

        return Convert.FromBase64String(s);
    }
}