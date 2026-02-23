using Mamey.Identity.Decentralized.Abstractions;
using Nethereum.Signer;

namespace Mamey.Identity.Decentralized.Services;

/// <summary>
/// Implements <see cref="IBlockchainProvider"/> using Nethereum for secp256k1/Ethereum, extendable for other chains.
/// </summary>
public class BlockchainProvider : IBlockchainProvider
{
    /// <inheritdoc />
    public Task<byte[]> SignAsync(byte[] payload, byte[] privateKey, string algorithm,
        CancellationToken cancellationToken = default)
    {
        if (algorithm.Equals("secp256k1", StringComparison.OrdinalIgnoreCase) ||
            algorithm.Equals("ES256K", StringComparison.OrdinalIgnoreCase))
        {
            // Sign with Ethereum (secp256k1)
            var key = new EthECKey(privateKey, true);
            var signer = new EthereumMessageSigner();
            var hash = signer.HashPrefixedMessage(payload);
            var signature = signer.Sign(hash, key);
            return Task.FromResult(System.Text.Encoding.UTF8.GetBytes(signature));
        }

        // Extend for Ed25519, etc.
        throw new NotSupportedException($"Algorithm '{algorithm}' is not yet supported by BlockchainProvider.");
    }

    /// <inheritdoc />
    public Task<bool> VerifyAsync(byte[] payload, byte[] signature, byte[] publicKey, string algorithm,
        CancellationToken cancellationToken = default)
    {
        if (algorithm.Equals("secp256k1", StringComparison.OrdinalIgnoreCase) ||
            algorithm.Equals("ES256K", StringComparison.OrdinalIgnoreCase))
        {
            // Verify Ethereum secp256k1 signature
            var signatureHex = System.Text.Encoding.UTF8.GetString(signature);
            var signer = new EthereumMessageSigner();
            var hash = signer.HashPrefixedMessage(payload);
            var payloadString = System.Text.Encoding.UTF8.GetString(payload);

            // Recovers the Ethereum address from signature and payload
            var address = signer.EncodeUTF8AndEcRecover(payloadString, signatureHex);

            // Derives the expected address from the public key
            var expectedAddress = GetEthereumAddressFromPublicKey(publicKey);

            return Task.FromResult(
                address != null &&
                expectedAddress != null &&
                address.Equals(expectedAddress, StringComparison.OrdinalIgnoreCase));
        }

        // Extend for Ed25519, etc.
        throw new NotSupportedException($"Algorithm '{algorithm}' is not yet supported by BlockchainProvider.");
    }

    /// <inheritdoc />
    public Task<string> GetAddressAsync(byte[] publicKey, string network, CancellationToken cancellationToken = default)
    {
        if (string.Equals(network, "ethereum", StringComparison.OrdinalIgnoreCase))
        {
            var key = new EthECKey(publicKey, false); // false = public key only
            return Task.FromResult(key.GetPublicAddress());
        }

        // Extend for Bitcoin, Solana, etc.
        throw new NotSupportedException($"Network '{network}' is not yet supported by BlockchainProvider.");
    }

    /// <inheritdoc />
    public Task<bool> VerifyOnChainAsync(string didOrCredentialId, CancellationToken cancellationToken = default)
    {
        // Stub for demo: always returns true. Extend with ENS, registry, smart contract, or block explorer checks.
        return Task.FromResult(true);
    }

    /// <summary>
    /// Derives the Ethereum address from an uncompressed secp256k1 public key.
    /// </summary>
    /// <param name="publicKey">Raw secp256k1 public key bytes (uncompressed).</param>
    /// <returns>Ethereum address (0x... hex string).</returns>
    private string GetEthereumAddressFromPublicKey(byte[] publicKey)
    {
        var key = new EthECKey(publicKey, false); // false = public key only
        return key.GetPublicAddress();
    }
}