namespace Mamey.Auth.DecentralizedIdentifiers.Crypto;

/// <summary>
/// Example HSM key provider for Azure Key Vault.
/// </summary>
public class AzureKeyVaultHsmKeyProvider : IHsmKeyProvider
{
#if AZURE_KEYVAULT
        public async Task<byte[]> SignAsync(string keyId, byte[] payload, string algorithm, CancellationToken cancellationToken
 = default)
        {
            var client = new CryptographyClient(new Uri(keyId), new DefaultAzureCredential());
            var alg = MapAlgorithm(algorithm);
            var result = await client.SignDataAsync(alg, payload, cancellationToken: cancellationToken);
            return result.Signature;
        }

        public async Task<bool> VerifyAsync(string keyId, byte[] payload, byte[] signature, string algorithm, CancellationToken cancellationToken
 = default)
        {
            var client = new CryptographyClient(new Uri(keyId), new DefaultAzureCredential());
            var alg = MapAlgorithm(algorithm);
            var result = await client.VerifyDataAsync(alg, payload, signature, cancellationToken: cancellationToken);
            return result.IsValid;
        }

        private static SignatureAlgorithm MapAlgorithm(string alg)
        {
            // Map from string to Azure SDK types (example)
            return alg switch
            {
                "ES256K" => SignatureAlgorithm.ES256K,
                "EdDSA"  => SignatureAlgorithm.EdDSA,
                "RS256"  => SignatureAlgorithm.RS256,
                _        => throw new NotSupportedException($"Unsupported algorithm: {alg}")
            };
        }
#else
    public Task<byte[]> SignAsync(string keyId, byte[] payload, string algorithm,
        CancellationToken cancellationToken = default)
        => throw new PlatformNotSupportedException("Azure Key Vault is not enabled in this build.");

    public Task<bool> VerifyAsync(string keyId, byte[] payload, byte[] signature, string algorithm,
        CancellationToken cancellationToken = default)
        => throw new PlatformNotSupportedException("Azure Key Vault is not enabled in this build.");
#endif
}