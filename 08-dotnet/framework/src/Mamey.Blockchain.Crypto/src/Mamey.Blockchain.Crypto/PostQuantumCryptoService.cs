using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Core;
using Mamey.Crypto;
using Mamey.Security.PostQuantum;
using Microsoft.Extensions.Logging;
using Mamey.Security.PostQuantum.Interfaces;
using Mamey.Security.PostQuantum.Models;
using Mamey.Security.PostQuantum.Native;

namespace Mamey.Blockchain.Crypto;

/// <summary>
/// gRPC service implementation exposing post-quantum cryptography operations
/// (ML-DSA signatures and hybrid modes) over the CryptoService contract.
///
/// This service focuses on PQC-specific algorithms and hybrid modes. Classical
/// algorithms (ed25519, secp256k1, RSA, etc.) are expected to be handled by the
/// existing classical CryptoService implementation.
/// </summary>
public sealed class PostQuantumCryptoService : CryptoService.CryptoServiceBase
{
    private readonly IPQKeyGenerator _keyGenerator;
    private readonly IPQSigner _signer;
    private readonly IHybridCryptoProvider _hybridCryptoProvider;
    private readonly ILogger<PostQuantumCryptoService> _logger;

    public PostQuantumCryptoService(
        IPQKeyGenerator keyGenerator,
        IPQSigner signer,
        IHybridCryptoProvider hybridCryptoProvider,
        ILogger<PostQuantumCryptoService> logger)
    {
        _keyGenerator = keyGenerator ?? throw new ArgumentNullException(nameof(keyGenerator));
        _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        _hybridCryptoProvider = hybridCryptoProvider ?? throw new ArgumentNullException(nameof(hybridCryptoProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task<GenerateKeypairResponse> GenerateKeypair(GenerateKeypairRequest request, ServerCallContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            if (string.IsNullOrWhiteSpace(request.KeyType))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "key_type is required"));
            }

            var (sigAlg, isSupported) = MapSignatureAlgorithm(request.KeyType);
            if (!isSupported)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Unsupported PQC key_type '{request.KeyType}'. Expected ml-dsa-44/65/87."));
            }

            var keyPair = _keyGenerator.GenerateKeyPair(sigAlg);

            var response = new GenerateKeypairResponse
            {
                PublicKey = Google.Protobuf.ByteString.CopyFrom(keyPair.PublicKey),
                PrivateKey = Google.Protobuf.ByteString.CopyFrom(keyPair.PrivateKey),
                KeyId = string.Empty,
                Success = true,
                ErrorMessage = string.Empty,
                AlgorithmUsed = request.KeyType
            };

            return Task.FromResult(response);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GenerateKeypair failed for key_type {KeyType}", request.KeyType);
            throw new RpcException(new Status(StatusCode.Internal, "Internal error while generating keypair."));
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation("GenerateKeypair({KeyType}) completed in {ElapsedMs} ms", request.KeyType, sw.ElapsedMilliseconds);
        }
    }

    public override Task<SignResponse> Sign(SignRequest request, ServerCallContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            if (request.Data is null || request.PrivateKey is null || string.IsNullOrWhiteSpace(request.Algorithm))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "data, private_key and algorithm are required."));
            }

            var data = request.Data.ToByteArray();
            var privateKey = request.PrivateKey.ToByteArray();

            var (sigAlg, isSupported) = MapSignatureAlgorithm(request.Algorithm);
            if (!isSupported)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Unsupported PQC algorithm '{request.Algorithm}'. Expected ml-dsa-44/65/87."));
            }

            // Currently this service focuses on ML-DSA signatures; hybrid mode is
            // surfaced via the separate HybridSigner model and higher-level
            // services. Here we emit a pure PQC signature.
            var (publicKeySize, privateKeySize, signatureSize) = GetMlDsaSizes(sigAlg);

            if (privateKey.Length != privateKeySize)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Invalid private key length for {request.Algorithm}. Expected {privateKeySize} bytes."));
            }

            var pqSignature = SignMlDsa(sigAlg, data, privateKey, signatureSize);

            var response = new SignResponse
            {
                ClassicalSignature = Google.Protobuf.ByteString.Empty,
                PqSignature = Google.Protobuf.ByteString.CopyFrom(pqSignature),
                AlgorithmUsed = request.Algorithm,
                Success = true,
                ErrorMessage = string.Empty
            };

            return Task.FromResult(response);
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation("Sign({Algorithm}) completed in {ElapsedMs} ms", request.Algorithm, sw.ElapsedMilliseconds);
        }
    }

    public override Task<VerifyResponse> Verify(VerifyRequest request, ServerCallContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            if (request.Data is null || request.Signature is null || request.PublicKey is null ||
                string.IsNullOrWhiteSpace(request.Algorithm))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    "data, signature, public_key and algorithm are required."));
            }

            var data = request.Data.ToByteArray();
            var signature = request.Signature.ToByteArray();
            var publicKey = request.PublicKey.ToByteArray();

            var (sigAlg, isSupported) = MapSignatureAlgorithm(request.Algorithm);
            if (!isSupported)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Unsupported PQC algorithm '{request.Algorithm}'. Expected ml-dsa-44/65/87."));
            }

            var (publicKeySize, _, _) = GetMlDsaSizes(sigAlg);
            if (publicKey.Length != publicKeySize)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Invalid public key length for {request.Algorithm}. Expected {publicKeySize} bytes."));
            }

            var verified = VerifyMlDsa(sigAlg, data, signature, publicKey);

            var response = new VerifyResponse
            {
                Verified = verified,
                Success = true,
                ErrorMessage = string.Empty,
                AlgorithmUsed = request.Algorithm
            };

            return Task.FromResult(response);
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation("Verify({Algorithm}) completed in {ElapsedMs} ms", request.Algorithm, sw.ElapsedMilliseconds);
        }
    }

    public override Task<GetSupportedAlgorithmsResponse> GetSupportedAlgorithms(GetSupportedAlgorithmsRequest request, ServerCallContext context)
    {
        var response = new GetSupportedAlgorithmsResponse();

        // PQC signature algorithms
        response.Algorithms.Add(new AlgorithmInfo
        {
            Name = "ml-dsa-44",
            Type = "signature",
            SecurityLevel = "NIST Level 1",
            PublicKeySize = 1312,
            PrivateKeySize = 2528,
            SignatureSize = 2420,
            QuantumResistant = true,
            NistStatus = "FIPS 204"
        });

        response.Algorithms.Add(new AlgorithmInfo
        {
            Name = "ml-dsa-65",
            Type = "signature",
            SecurityLevel = "NIST Level 3",
            PublicKeySize = 1952,
            PrivateKeySize = 4032,
            SignatureSize = 3309,
            QuantumResistant = true,
            NistStatus = "FIPS 204"
        });

        response.Algorithms.Add(new AlgorithmInfo
        {
            Name = "ml-dsa-87",
            Type = "signature",
            SecurityLevel = "NIST Level 5",
            PublicKeySize = 2592,
            PrivateKeySize = 4896,
            SignatureSize = 4627,
            QuantumResistant = true,
            NistStatus = "FIPS 204"
        });

        // Classical examples (treated as deprecated when include_deprecated == false)
        if (request.IncludeDeprecated)
        {
            response.Algorithms.Add(new AlgorithmInfo
            {
                Name = "rsa-2048",
                Type = "signature",
                SecurityLevel = "classical ~112-bit",
                PublicKeySize = 256,
                PrivateKeySize = 1190,
                SignatureSize = 256,
                QuantumResistant = false,
                NistStatus = "classical"
            });

            response.Algorithms.Add(new AlgorithmInfo
            {
                Name = "ecdsa-secp256k1",
                Type = "signature",
                SecurityLevel = "classical ~128-bit",
                PublicKeySize = 33,
                PrivateKeySize = 32,
                SignatureSize = 64,
                QuantumResistant = false,
                NistStatus = "classical"
            });
        }

        return Task.FromResult(response);
    }

    private static (SignatureAlgorithm algorithm, bool supported) MapSignatureAlgorithm(string algorithm)
    {
        if (string.IsNullOrWhiteSpace(algorithm))
        {
            return (default, false);
        }

        return algorithm.ToLowerInvariant() switch
        {
            "ml-dsa-44" => (SignatureAlgorithm.ML_DSA_44, true),
            "ml-dsa-65" => (SignatureAlgorithm.ML_DSA_65, true),
            "ml-dsa-87" => (SignatureAlgorithm.ML_DSA_87, true),
            _ => (default, false)
        };
    }

    private static (int publicKeySize, int privateKeySize, int signatureSize) GetMlDsaSizes(SignatureAlgorithm algorithm)
    {
        // Values aligned with liboqs constants (FIPS 204 / ML-DSA) - matching MLDSASigner.GetSizes
        return algorithm switch
        {
            SignatureAlgorithm.ML_DSA_44 => (1312, 2528, 2420),
            SignatureAlgorithm.ML_DSA_65 => (1952, 4000, 3293),
            SignatureAlgorithm.ML_DSA_87 => (2592, 4864, 4595),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Unsupported ML-DSA algorithm.")
        };
    }

    private static byte[] SignMlDsa(
        SignatureAlgorithm algorithm,
        byte[] message,
        byte[] secretKey,
        int maxSignatureSize)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));
        if (secretKey is null) throw new ArgumentNullException(nameof(secretKey));

        var signature = new byte[maxSignatureSize];
        ulong sigLen = (ulong)signature.Length;

        int status = algorithm switch
        {
            SignatureAlgorithm.ML_DSA_44 => LibOQS.OQS_SIG_dilithium_2_sign(
                signature, ref sigLen, message, (ulong)message.Length, secretKey),
            SignatureAlgorithm.ML_DSA_65 => LibOQS.OQS_SIG_dilithium_3_sign(
                signature, ref sigLen, message, (ulong)message.Length, secretKey),
            SignatureAlgorithm.ML_DSA_87 => LibOQS.OQS_SIG_dilithium_5_sign(
                signature, ref sigLen, message, (ulong)message.Length, secretKey),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Unsupported ML-DSA algorithm.")
        };

        LibOQS.EnsureSuccess(status, $"OQS_SIG_{algorithm}_sign");

        if (sigLen == (ulong)signature.Length)
        {
            return signature;
        }

        var trimmed = new byte[sigLen];
        Buffer.BlockCopy(signature, 0, trimmed, 0, (int)sigLen);
        return trimmed;
    }

    private static bool VerifyMlDsa(
        SignatureAlgorithm algorithm,
        byte[] message,
        byte[] signature,
        byte[] publicKey)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));
        if (signature is null) throw new ArgumentNullException(nameof(signature));
        if (publicKey is null) throw new ArgumentNullException(nameof(publicKey));

        ulong sigLen = (ulong)signature.Length;

        int status = algorithm switch
        {
            SignatureAlgorithm.ML_DSA_44 => LibOQS.OQS_SIG_dilithium_2_verify(
                message, (ulong)message.Length, signature, sigLen, publicKey),
            SignatureAlgorithm.ML_DSA_65 => LibOQS.OQS_SIG_dilithium_3_verify(
                message, (ulong)message.Length, signature, sigLen, publicKey),
            SignatureAlgorithm.ML_DSA_87 => LibOQS.OQS_SIG_dilithium_5_verify(
                message, (ulong)message.Length, signature, sigLen, publicKey),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, "Unsupported ML-DSA algorithm.")
        };

        // For verification we return bool instead of throwing; non-zero means verification failed.
        return status == 0;
    }
}


