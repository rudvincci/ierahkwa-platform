namespace Mamey.Security.Tests.Shared.Utilities;

/// <summary>
/// Provides test encryption keys for testing.
/// </summary>
public static class TestKeys
{
    /// <summary>
    /// Valid AES-256 key (32 characters).
    /// </summary>
    public const string ValidAesKey = "12345678901234567890123456789012";

    /// <summary>
    /// Valid TripleDES key (24 characters).
    /// </summary>
    public const string ValidTripleDesKey = "123456789012345678901234";

    /// <summary>
    /// Invalid key (too short).
    /// </summary>
    public const string InvalidKey = "tooshort";

    /// <summary>
    /// Empty key.
    /// </summary>
    public const string EmptyKey = "";

    /// <summary>
    /// Valid AES-256 key as byte array.
    /// </summary>
    public static byte[] ValidAesKeyBytes => System.Text.Encoding.UTF8.GetBytes(ValidAesKey);

    /// <summary>
    /// Valid TripleDES key as byte array.
    /// </summary>
    public static byte[] ValidTripleDesKeyBytes => System.Text.Encoding.UTF8.GetBytes(ValidTripleDesKey);

    /// <summary>
    /// Valid AES IV (16 bytes).
    /// </summary>
    public static byte[] ValidAesIv => new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

    /// <summary>
    /// Valid TripleDES IV (8 bytes).
    /// </summary>
    public static byte[] ValidTripleDesIv => new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

    /// <summary>
    /// Invalid IV (wrong length).
    /// </summary>
    public static byte[] InvalidIv => new byte[] { 1, 2, 3 };
}



