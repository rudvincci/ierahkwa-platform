using System.Threading;
using System.Threading.Tasks;
using Mamey.Security.PostQuantum.Algorithms.MLDSA;
using Mamey.Security.PostQuantum.Interfaces;
using Mamey.Security.PostQuantum.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Security.PostQuantum.Extensions;

/// <summary>
/// DI registration helpers for the post-quantum security layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostQuantumSecurity(this IServiceCollection services)
    {
        // Register ML-DSA signer with a default ML-DSA-65 key pair.
        services.AddSingleton<IPQSigner>(_ =>
        {
            var keyPair = MLDSASigner.GenerateKeyPair(SignatureAlgorithm.ML_DSA_65);
            return new MLDSASigner(SignatureAlgorithm.ML_DSA_65, keyPair);
        });

        // Key generator can be extended for additional algorithms as needed.
        services.AddSingleton<IPQKeyGenerator, DefaultPQKeyGenerator>();

        // Register ML-KEM encryptor and hybrid signer.
        services.AddSingleton<IPQEncryptor, Algorithms.MLKEM.MLKEMEncryptor>();
        services.AddSingleton<IHybridCryptoProvider>(_ => new Hybrid.HybridSigner(SignatureAlgorithm.HYBRID_RSA_MLDSA65));
        return services;
    }

    private sealed class DefaultPQKeyGenerator : IPQKeyGenerator
    {
        public PQKeyPair GenerateKeyPair(SignatureAlgorithm algorithm) =>
            algorithm switch
            {
                SignatureAlgorithm.ML_DSA_44 or SignatureAlgorithm.ML_DSA_65 or SignatureAlgorithm.ML_DSA_87
                    => MLDSASigner.GenerateKeyPair(algorithm),
                _ => new PQKeyPair(Array.Empty<byte>(), Array.Empty<byte>())
            };

        public PQKeyPair GenerateKeyPair(KemAlgorithm algorithm) =>
            new(Array.Empty<byte>(), Array.Empty<byte>());

        public Task<PQKeyPair> GenerateKeyPairAsync(SignatureAlgorithm algorithm, CancellationToken cancellationToken = default) =>
            Task.FromResult(GenerateKeyPair(algorithm));

        public bool ValidateKeyPair(PQKeyPair keyPair) =>
            keyPair.PublicKey.Length > 0 && keyPair.PrivateKey.Length > 0;
    }
}



