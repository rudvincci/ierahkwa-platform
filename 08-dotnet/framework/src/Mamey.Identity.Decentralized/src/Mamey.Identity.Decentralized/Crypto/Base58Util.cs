using SimpleBase;

namespace Mamey.Identity.Decentralized.Crypto;

/// <summary>
/// Provides base58 encoding/decoding (Bitcoin alphabet).
/// </summary>
public static class Base58Util
{
    public static string Encode(byte[] input) => Base58.Bitcoin.Encode(input);

    public static byte[] Decode(string input) => Base58.Bitcoin.Decode(input);
}