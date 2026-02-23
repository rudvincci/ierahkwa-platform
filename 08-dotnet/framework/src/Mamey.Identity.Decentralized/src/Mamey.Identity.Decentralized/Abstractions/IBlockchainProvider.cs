namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Provides blockchain interactions for signing, verification, and address operations.
/// Used for credential signature, DID registry checks, and blockchain-resident proofs.
/// </summary>
public interface IBlockchainProvider
{
    /// <summary>
    /// Signs a payload with the given private key and returns a signature.
    /// </summary>
    /// <param name="payload">The data to sign.</param>
    /// <param name="privateKey">Raw private key bytes.</param>
    /// <param name="algorithm">Signing algorithm (e.g., "secp256k1", "Ed25519").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Signature bytes.</returns>
    Task<byte[]> SignAsync(byte[] payload, byte[] privateKey, string algorithm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a signature with a public key.
    /// </summary>
    /// <param name="payload">Original data.</param>
    /// <param name="signature">Signature bytes.</param>
    /// <param name="publicKey">Raw public key bytes.</param>
    /// <param name="algorithm">Algorithm used.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if signature is valid; otherwise false.</returns>
    Task<bool> VerifyAsync(byte[] payload, byte[] signature, byte[] publicKey, string algorithm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves the blockchain address for a given public key (e.g., Ethereum address).
    /// </summary>
    /// <param name="publicKey">Raw public key bytes.</param>
    /// <param name="network">Blockchain/network (e.g., "ethereum").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Network-specific address as string.</returns>
    Task<string> GetAddressAsync(byte[] publicKey, string network, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies on-chain state for a given DID or credential (e.g., revocation, key, registry).
    /// </summary>
    /// <param name="didOrCredentialId">DID or credential ID to verify.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if valid on chain, otherwise false.</returns>
    Task<bool> VerifyOnChainAsync(string didOrCredentialId, CancellationToken cancellationToken = default);
}