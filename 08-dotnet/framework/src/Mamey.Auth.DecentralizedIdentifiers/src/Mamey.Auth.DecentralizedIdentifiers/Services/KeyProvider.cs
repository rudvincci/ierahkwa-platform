using System.Collections.Concurrent;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Services;

/// <summary>
/// In-memory implementation of <see cref="IKeyProvider"/> for demo/testing purposes.
/// In production, replace or extend with HSM, Vault, or distributed KMS integration.
/// </summary>
public class KeyProvider : IKeyProvider
{
    // Thread-safe in-memory stores for public/private keys.
    private readonly ConcurrentDictionary<string, byte[]> _publicKeys = new();
    private readonly ConcurrentDictionary<string, byte[]> _privateKeys = new();

    /// <summary>
    /// Registers a public key in memory for quick lookup.
    /// </summary>
    /// <param name="keyId">The key or verification method ID.</param>
    /// <param name="keyBytes">Raw public key bytes.</param>
    public void RegisterPublicKey(string keyId, byte[] keyBytes)
        => _publicKeys[keyId] = keyBytes;

    /// <summary>
    /// Registers a private key in memory for quick lookup.
    /// </summary>
    /// <param name="keyId">The key or DID.</param>
    /// <param name="keyBytes">Raw private key bytes.</param>
    public void RegisterPrivateKey(string keyId, byte[] keyBytes)
        => _privateKeys[keyId] = keyBytes;

    /// <inheritdoc />
    public Task<byte[]> GetPublicKeyAsync(string verificationMethodIdOrDid,
        CancellationToken cancellationToken = default)
    {
        if (_publicKeys.TryGetValue(verificationMethodIdOrDid, out var key))
            return Task.FromResult(key);

        // In a real system, try to resolve via DID doc, or from a secure store.
        throw new KeyNotFoundException($"Public key for '{verificationMethodIdOrDid}' not found.");
    }

    /// <inheritdoc />
    public Task<byte[]> GetPrivateKeyAsync(string keyIdOrDid, CancellationToken cancellationToken = default)
    {
        if (_privateKeys.TryGetValue(keyIdOrDid, out var key))
            return Task.FromResult(key);

        // WARNING: Only expose private keys if safe to do so!
        throw new KeyNotFoundException($"Private key for '{keyIdOrDid}' not found.");
    }

    /// <inheritdoc />
    public byte[] ResolveKey(string keyIdOrDid)
    {
        if (_publicKeys.TryGetValue(keyIdOrDid, out var pub))
            return pub;
        if (_privateKeys.TryGetValue(keyIdOrDid, out var priv))
            return priv;
        return null;
    }

    /// <inheritdoc />
    public async Task<bool> VerifyAsync(byte[] data, byte[] signature, string keyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var publicKey = await GetPublicKeyAsync(keyId, cancellationToken);
            // This is a placeholder implementation - in a real system, you would use a proper cryptographic library
            // to verify the signature using the public key
            return true; // Placeholder - always returns true for now
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<byte[]> SignAsync(byte[] data, string keyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var privateKey = await GetPrivateKeyAsync(keyId, cancellationToken);
            // This is a placeholder implementation - in a real system, you would use a proper cryptographic library
            // to sign the data using the private key
            return new byte[64]; // Placeholder - returns dummy signature for now
        }
        catch
        {
            throw new InvalidOperationException($"Failed to sign data with key '{keyId}'");
        }
    }
}