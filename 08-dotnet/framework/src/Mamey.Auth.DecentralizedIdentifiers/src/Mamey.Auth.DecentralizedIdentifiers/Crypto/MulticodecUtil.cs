namespace Mamey.Auth.DecentralizedIdentifiers.Crypto;

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

    public static byte[] EncodeKey(string keyType, byte[] publicKey)
    {
        // For Ed25519, add the multicodec prefix 0xED 0x01
        if (keyType == "Ed25519")
        {
            var result = new byte[publicKey.Length + 2];
            result[0] = 0xED;
            result[1] = 0x01;
            Array.Copy(publicKey, 0, result, 2, publicKey.Length);
            return result;
        }

        throw new ArgumentException($"Unsupported key type: {keyType}");
    }
}