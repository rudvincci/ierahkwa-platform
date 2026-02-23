using Mamey.Identity.Decentralized.Crypto;

namespace Mamey.Identity.Decentralized.Methods.Key;

/// <summary>
/// Utilities for parsing did:key multicodec keys.
/// </summary>
internal static class MulticodecKeyParser
{
    public static (string keyType, byte[] publicKey, string codec) Parse(string multibase)
    {
        // Validate multibase (e.g., "z...")
        if (string.IsNullOrWhiteSpace(multibase) || multibase[0] != 'z')
            throw new ArgumentException("Method-specific id must be multibase encoded (starts with 'z').");

        // Remove multibase prefix and decode multicodec (e.g., for Ed25519, 0xed01)
        var bytes = MultibaseUtil.Decode(multibase);
        var (codec, keyType) = MulticodecUtil.GetKeyType(bytes, out var keyBytes);

        return (keyType, keyBytes, codec);
    }
}