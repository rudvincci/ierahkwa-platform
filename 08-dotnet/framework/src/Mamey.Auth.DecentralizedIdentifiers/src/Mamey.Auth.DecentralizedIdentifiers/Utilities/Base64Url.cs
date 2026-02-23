namespace Mamey.Auth.DecentralizedIdentifiers.Utilities;

/// <summary>
/// Utilities for base64url encoding and decoding (RFC 7515, RFC 4648).
/// </summary>
public static class Base64Url
{
    /// <summary>
    /// Encodes the input bytes to base64url (no padding, URL safe).
    /// </summary>
    public static string Encode(byte[] input)
    {
        if (input == null) return null;
        var base64 = Convert.ToBase64String(input); // Regular base64 with padding
        return base64
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    /// <summary>
    /// Decodes a base64url string (no padding, URL safe) to bytes.
    /// </summary>
    public static byte[] Decode(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Array.Empty<byte>();
        string s = input.Replace('-', '+').Replace('_', '/');
        // Pad with '=' as needed
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }
        return Convert.FromBase64String(s);
    }
}