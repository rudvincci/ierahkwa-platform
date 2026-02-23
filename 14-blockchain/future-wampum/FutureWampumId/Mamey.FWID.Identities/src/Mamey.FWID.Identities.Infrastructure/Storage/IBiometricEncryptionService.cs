namespace Mamey.FWID.Identities.Infrastructure.Storage;

/// <summary>
/// Service for encrypting and decrypting biometric data using AES-256-GCM.
/// Uses Vault-managed encryption keys for secure key management.
/// </summary>
internal interface IBiometricEncryptionService
{
    /// <summary>
    /// Encrypts biometric data using AES-256-GCM.
    /// </summary>
    /// <param name="data">Plaintext biometric data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Encrypted data with IV and authentication tag</returns>
    Task<EncryptedBiometricData> EncryptAsync(byte[] data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrypts biometric data using AES-256-GCM.
    /// </summary>
    /// <param name="encryptedData">Encrypted data with IV and authentication tag</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Decrypted plaintext biometric data</returns>
    Task<byte[]> DecryptAsync(EncryptedBiometricData encryptedData, CancellationToken cancellationToken = default);
}

/// <summary>
/// Encrypted biometric data structure containing ciphertext, IV, and authentication tag.
/// </summary>
public class EncryptedBiometricData
{
    public byte[] Ciphertext { get; set; } = null!;
    public byte[] IV { get; set; } = null!;
    public byte[] Tag { get; set; } = null!;
    public int Version { get; set; } = 1;
}
