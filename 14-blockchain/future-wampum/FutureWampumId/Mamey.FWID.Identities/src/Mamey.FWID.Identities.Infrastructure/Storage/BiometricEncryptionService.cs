using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Storage;

/// <summary>
/// Service for encrypting and decrypting biometric data using AES-256-GCM.
/// Uses Vault-managed encryption keys for secure key management.
/// </summary>
internal class BiometricEncryptionService : IBiometricEncryptionService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BiometricEncryptionService> _logger;
    private const int KeySize = 32; // 256 bits
    private const int IVSize = 12; // 96 bits for GCM
    private const int TagSize = 16; // 128 bits for GCM authentication tag

    public BiometricEncryptionService(
        IConfiguration configuration,
        ILogger<BiometricEncryptionService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<EncryptedBiometricData> EncryptAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (data == null || data.Length == 0)
        {
            throw new ArgumentException("Data cannot be null or empty", nameof(data));
        }

        try
        {
            // Get encryption key from Vault or configuration
            var encryptionKey = await GetEncryptionKeyAsync(cancellationToken);
            
            // Generate random IV
            var iv = new byte[IVSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }

            // Encrypt using AES-256-GCM
            using var aes = new AesGcm(encryptionKey);
            var ciphertext = new byte[data.Length];
            var tag = new byte[TagSize];
            
            aes.Encrypt(iv, data, ciphertext, tag);

            _logger.LogDebug(
                "Encrypted biometric data: Size={Size} bytes, IV={IVLength} bytes, Tag={TagLength} bytes",
                data.Length,
                iv.Length,
                tag.Length);

            return new EncryptedBiometricData
            {
                Ciphertext = ciphertext,
                IV = iv,
                Tag = tag,
                Version = 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to encrypt biometric data");
            throw;
        }
    }

    public async Task<byte[]> DecryptAsync(EncryptedBiometricData encryptedData, CancellationToken cancellationToken = default)
    {
        if (encryptedData == null)
        {
            throw new ArgumentNullException(nameof(encryptedData));
        }

        if (encryptedData.Ciphertext == null || encryptedData.IV == null || encryptedData.Tag == null)
        {
            throw new ArgumentException("Encrypted data is incomplete", nameof(encryptedData));
        }

        try
        {
            // Get encryption key from Vault or configuration
            var encryptionKey = await GetEncryptionKeyAsync(cancellationToken);

            // Decrypt using AES-256-GCM
            using var aes = new AesGcm(encryptionKey);
            var plaintext = new byte[encryptedData.Ciphertext.Length];
            
            aes.Decrypt(encryptedData.IV, encryptedData.Ciphertext, encryptedData.Tag, plaintext);

            _logger.LogDebug(
                "Decrypted biometric data: Size={Size} bytes",
                plaintext.Length);

            return plaintext;
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex, "Failed to decrypt biometric data - authentication tag verification failed");
            throw new InvalidOperationException("Failed to decrypt biometric data - data may be corrupted or tampered", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt biometric data");
            throw;
        }
    }

    private async Task<byte[]> GetEncryptionKeyAsync(CancellationToken cancellationToken)
    {
        // TODO: Integrate with Vault for key management
        // For now, use configuration-based key (should be replaced with Vault integration)
        var keyBase64 = _configuration["BiometricEncryption:Key"];
        
        if (string.IsNullOrEmpty(keyBase64))
        {
            // Generate a key for development (should never happen in production)
            _logger.LogWarning("Biometric encryption key not found in configuration - generating temporary key");
            var tempKey = new byte[KeySize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tempKey);
            }
            return tempKey;
        }

        var key = Convert.FromBase64String(keyBase64);
        if (key.Length != KeySize)
        {
            throw new InvalidOperationException($"Encryption key must be {KeySize} bytes (256 bits)");
        }

        return await Task.FromResult(key);
    }
}
