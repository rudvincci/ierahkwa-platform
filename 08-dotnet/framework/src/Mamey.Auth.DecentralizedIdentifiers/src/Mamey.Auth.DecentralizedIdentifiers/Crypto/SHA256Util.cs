using System.Security.Cryptography;

namespace Mamey.Auth.DecentralizedIdentifiers.Crypto;

/// <summary>
/// Utility for SHA256 hashing.
/// </summary>
public static class SHA256Util
{
    public static byte[] Hash(byte[] data)
    {
        using var sha = SHA256.Create();
        return sha.ComputeHash(data);
    }
}