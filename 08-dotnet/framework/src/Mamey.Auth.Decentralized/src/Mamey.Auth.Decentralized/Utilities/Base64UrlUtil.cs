namespace Mamey.Auth.Decentralized.Utilities;

/// <summary>
/// Utility class for Base64URL encoding and decoding
/// </summary>
public static class Base64UrlUtil
{
    /// <summary>
    /// Encodes bytes to Base64URL string
    /// </summary>
    /// <param name="data">The data to encode</param>
    /// <returns>Base64URL encoded string</returns>
    public static string Encode(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        
        var base64 = Convert.ToBase64String(data);
        return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }
    
    /// <summary>
    /// Decodes Base64URL string to bytes
    /// </summary>
    /// <param name="base64Url">The Base64URL string to decode</param>
    /// <returns>Decoded bytes</returns>
    public static byte[] Decode(string base64Url)
    {
        if (string.IsNullOrEmpty(base64Url))
            throw new ArgumentException("Base64URL string cannot be null or empty", nameof(base64Url));
        
        // Add padding if needed
        var padding = base64Url.Length % 4;
        if (padding != 0)
        {
            base64Url += new string('=', 4 - padding);
        }
        
        // Convert Base64URL to Base64
        var base64 = base64Url.Replace('-', '+').Replace('_', '/');
        
        return Convert.FromBase64String(base64);
    }
    
    /// <summary>
    /// Encodes a string to Base64URL
    /// </summary>
    /// <param name="text">The text to encode</param>
    /// <returns>Base64URL encoded string</returns>
    public static string EncodeString(string text)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));
        
        var bytes = Encoding.UTF8.GetBytes(text);
        return Encode(bytes);
    }
    
    /// <summary>
    /// Decodes Base64URL string to text
    /// </summary>
    /// <param name="base64Url">The Base64URL string to decode</param>
    /// <returns>Decoded text</returns>
    public static string DecodeString(string base64Url)
    {
        var bytes = Decode(base64Url);
        return Encoding.UTF8.GetString(bytes);
    }
}
