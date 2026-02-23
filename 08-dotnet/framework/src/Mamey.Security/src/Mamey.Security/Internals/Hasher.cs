using System.Security.Cryptography;
using System.Text;

namespace Mamey.Security.Internals;

/// <summary>
/// Provides SHA-512 hashing utilities.
/// </summary>
internal sealed class Hasher : IHasher
{
    /// <summary>
    /// Hashes the input string using SHA-512 and returns a hex-encoded string.
    /// </summary>
    public string Hash(string data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (string.IsNullOrWhiteSpace(data) && data != string.Empty)
            throw new ArgumentNullException(nameof(data));
        using var sha512 = SHA512.Create();
        var bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(data));
        var builder = new StringBuilder();
        foreach (var @byte in bytes)
        {
            builder.Append(@byte.ToString("x2"));
        }

        return builder.ToString();
    }
    

    /// <summary>
    /// Hashes the input byte array using SHA-512 and returns the raw hash bytes.
    /// </summary>
    public byte[] Hash(byte[] data)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentNullException(nameof(data));

        using var sha512 = SHA512.Create();
        return sha512.ComputeHash(data);
    }
    /// <summary>
    /// Hashes the input string using SHA-512 and returns the raw hash bytes.
    /// </summary>
    public byte[] HashToBytes(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentNullException(nameof(data));

        return Hash(Encoding.UTF8.GetBytes(data));
    }
}
