namespace Mamey.Identity.Decentralized.Serialization;

/// <summary>
/// Provides multibase encoding/decoding for string representations (delegates to Crypto for real work).
/// </summary>
public static class MultiBaseEncoder
{
    /// <summary>
    /// Encodes to the given multibase type (e.g., base58btc).
    /// </summary>
    public static string EncodeBase58(byte[] input)
        => Crypto.MultibaseUtil.Encode(input, Crypto.MultibaseUtil.Base.Base58Btc);

    /// <summary>
    /// Decodes a base58btc multibase string.
    /// </summary>
    public static byte[] DecodeBase58(string input)
        => Crypto.MultibaseUtil.Decode(input);
}