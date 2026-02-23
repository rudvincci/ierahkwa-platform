namespace Mamey.Identity.Decentralized.Crypto;

/// <summary>
/// Utility class for Multibase encoding/decoding.
/// </summary>
public static class MultibaseUtil
{
    public enum Base
    {
        Base58Btc = 'z'
    }

    public static string Encode(byte[] data, Base multibase)
    {
        switch (multibase)
        {
            case Base.Base58Btc: return "z" + Base58Util.Encode(data);
            default: throw new NotSupportedException("Multibase not supported.");
        }
    }

    public static byte[] Decode(string multibaseString)
    {
        if (string.IsNullOrEmpty(multibaseString))
            throw new ArgumentNullException(nameof(multibaseString));

        char prefix = multibaseString[0];
        switch (prefix)
        {
            case 'z': return Base58Util.Decode(multibaseString[1..]);
            default: throw new NotSupportedException($"Unknown multibase prefix: {prefix}");
        }
    }
}