using System.Security.Cryptography;

namespace Mamey.Auth.DecentralizedIdentifiers.Utilities;

/// <summary>
/// Utility methods for key normalization, fingerprinting, and simple conversions.
/// </summary>
public static class KeyUtils
{
    /// <summary>
    /// Computes a SHA256 fingerprint for a public key.
    /// </summary>
    public static string ComputeFingerprint(byte[] key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(key);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Encodes key bytes to a base64url string.
    /// </summary>
    public static string ToBase64Url(byte[] key)
    {
        return Convert.ToBase64String(key)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}