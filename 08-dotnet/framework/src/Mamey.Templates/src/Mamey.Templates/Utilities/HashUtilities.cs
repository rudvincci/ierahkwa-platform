using System.Security.Cryptography;

namespace Mamey.Templates.Utilities;

internal static class HashUtilities
{
    public static string Sha256Hex(ReadOnlySpan<byte> data)
        => Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();
}