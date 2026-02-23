using SimpleBase;

namespace Mamey.Auth.Decentralized.Utilities;

/// <summary>
/// Utility class for Base58 encoding and decoding
/// </summary>
public static class Base58Util
{
    /// <summary>
    /// Encodes bytes to Base58 string
    /// </summary>
    /// <param name="data">The data to encode</param>
    /// <returns>Base58 encoded string</returns>
    public static string Encode(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        
        return Base58.Bitcoin.Encode(data);
    }
    
    /// <summary>
    /// Decodes Base58 string to bytes
    /// </summary>
    /// <param name="base58">The Base58 string to decode</param>
    /// <returns>Decoded bytes</returns>
    public static byte[] Decode(string base58)
    {
        if (string.IsNullOrEmpty(base58))
            throw new ArgumentException("Base58 string cannot be null or empty", nameof(base58));
        
        return Base58.Bitcoin.Decode(base58);
    }
    
    /// <summary>
    /// Encodes a string to Base58
    /// </summary>
    /// <param name="text">The text to encode</param>
    /// <returns>Base58 encoded string</returns>
    public static string EncodeString(string text)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));
        
        var bytes = Encoding.UTF8.GetBytes(text);
        return Encode(bytes);
    }
    
    /// <summary>
    /// Decodes Base58 string to text
    /// </summary>
    /// <param name="base58">The Base58 string to decode</param>
    /// <returns>Decoded text</returns>
    public static string DecodeString(string base58)
    {
        var bytes = Decode(base58);
        return Encoding.UTF8.GetString(bytes);
    }
    
    /// <summary>
    /// Checks if a string is valid Base58
    /// </summary>
    /// <param name="base58">The string to check</param>
    /// <returns>True if valid Base58</returns>
    public static bool IsValid(string base58)
    {
        if (string.IsNullOrEmpty(base58))
            return false;
        
        try
        {
            Decode(base58);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
