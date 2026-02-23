using SimpleBase;

namespace Mamey.Auth.Decentralized.Utilities;

/// <summary>
/// Utility class for Multibase encoding and decoding
/// </summary>
public static class MultibaseUtil
{
    /// <summary>
    /// Encodes bytes to multibase string
    /// </summary>
    /// <param name="data">The data to encode</param>
    /// <param name="encoding">The multibase encoding to use (default: base58btc)</param>
    /// <returns>Multibase encoded string</returns>
    public static string Encode(byte[] data, char encoding = 'z')
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        
        return encoding switch
        {
            'z' => 'z' + Base58.Bitcoin.Encode(data),
            'Z' => 'Z' + Base58.Bitcoin.Encode(data),
            'm' => 'm' + Convert.ToBase64String(data),
            'M' => 'M' + Convert.ToBase64String(data),
            'u' => 'u' + Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_').TrimEnd('='),
            'U' => 'U' + Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_').TrimEnd('='),
            'b' => 'b' + Convert.ToBase64String(data),
            'B' => 'B' + Convert.ToBase64String(data),
            _ => throw new ArgumentException($"Unsupported multibase encoding: {encoding}", nameof(encoding))
        };
    }
    
    /// <summary>
    /// Decodes multibase string to bytes
    /// </summary>
    /// <param name="multibase">The multibase string to decode</param>
    /// <returns>Decoded bytes</returns>
    public static byte[] Decode(string multibase)
    {
        if (string.IsNullOrEmpty(multibase))
            throw new ArgumentException("Multibase string cannot be null or empty", nameof(multibase));
        
        var encoding = multibase[0];
        var data = multibase[1..];
        
        return encoding switch
        {
            'z' or 'Z' => Base58.Bitcoin.Decode(data),
            'm' or 'M' or 'b' or 'B' => Convert.FromBase64String(data),
            'u' or 'U' => DecodeBase64Url(data),
            _ => throw new ArgumentException($"Unsupported multibase encoding: {encoding}", nameof(multibase))
        };
    }
    
    /// <summary>
    /// Gets the encoding character from a multibase string
    /// </summary>
    /// <param name="multibase">The multibase string</param>
    /// <returns>The encoding character</returns>
    public static char GetEncoding(string multibase)
    {
        if (string.IsNullOrEmpty(multibase))
            throw new ArgumentException("Multibase string cannot be null or empty", nameof(multibase));
        
        return multibase[0];
    }
    
    /// <summary>
    /// Checks if a string is valid multibase
    /// </summary>
    /// <param name="multibase">The string to check</param>
    /// <returns>True if valid multibase, false otherwise</returns>
    public static bool IsValid(string multibase)
    {
        if (string.IsNullOrEmpty(multibase) || multibase.Length < 2)
            return false;
        
        var encoding = multibase[0];
        var data = multibase[1..];
        
        return encoding switch
        {
            'z' or 'Z' => IsValidBase58(data),
            'm' or 'M' or 'b' or 'B' => IsValidBase64(data),
            'u' or 'U' => IsValidBase64Url(data),
            _ => false
        };
    }
    
    /// <summary>
    /// Decodes Base64URL string to bytes
    /// </summary>
    /// <param name="base64Url">The Base64URL string to decode</param>
    /// <returns>Decoded bytes</returns>
    private static byte[] DecodeBase64Url(string base64Url)
    {
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
    /// Checks if a string is valid Base58
    /// </summary>
    /// <param name="base58">The string to check</param>
    /// <returns>True if valid Base58, false otherwise</returns>
    private static bool IsValidBase58(string base58)
    {
        try
        {
            Base58.Bitcoin.Decode(base58);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Checks if a string is valid Base64
    /// </summary>
    /// <param name="base64">The string to check</param>
    /// <returns>True if valid Base64, false otherwise</returns>
    private static bool IsValidBase64(string base64)
    {
        try
        {
            Convert.FromBase64String(base64);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Checks if a string is valid Base64URL
    /// </summary>
    /// <param name="base64Url">The string to check</param>
    /// <returns>True if valid Base64URL, false otherwise</returns>
    private static bool IsValidBase64Url(string base64Url)
    {
        try
        {
            DecodeBase64Url(base64Url);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
