namespace Mamey.Identity.Decentralized.Crypto;

/// <summary>
/// Utility for mapping multicodec prefixes to key types.
/// </summary>
public static class MulticodecUtil
{
    // Example: Ed25519 pubkey codec = 0xED01
    public static (string codec, string keyType) GetKeyType(byte[] bytes, out byte[] keyBytes)
    {
        // For Ed25519, first two bytes: 0xED 0x01
        if (bytes.Length > 2 && bytes[0] == 0xED && bytes[1] == 0x01)
        {
            keyBytes = new byte[bytes.Length - 2];
            Array.Copy(bytes, 2, keyBytes, 0, bytes.Length - 2);
            return ("ed25519-pub", "Ed25519");
        }

        throw new ArgumentException("Unsupported or unknown multicodec key type.");
    }
}