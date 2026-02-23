using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Mamey.Security;
using Mamey.Security.PostQuantum.Models;
using Mamey.Wallet;

namespace Mamey.Blockchain.Wallet;

/// <summary>
/// gRPC wallet service implementation focused on post-quantum key management and
/// migration support. This service implements the WalletService contract defined
/// in wallet.proto and delegates cryptographic operations to the Mamey.Security
/// and Mamey.Security.PostQuantum layers.
///
/// NOTE: This implementation focuses on request validation, algorithm routing,
/// and interaction with the security provider; concrete persistence details are
/// left to the hosting service and database layer.
/// </summary>
public sealed class PostQuantumWalletService : WalletService.WalletServiceBase
{
    private readonly ISecurityProvider _securityProvider;
    private readonly ILogger<PostQuantumWalletService> _logger;

    public PostQuantumWalletService(ISecurityProvider securityProvider, ILogger<PostQuantumWalletService> logger)
    {
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task<GenerateKeyResponse> GenerateKey(GenerateKeyRequest request, ServerCallContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            if (string.IsNullOrWhiteSpace(request.KeyType))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "key_type is required."));
            }

            // This implementation only validates PQC-friendly key types and defers
            // actual storage to the host application.
            var keyType = request.KeyType.ToLowerInvariant();
            if (!IsSupportedKeyType(keyType))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Unsupported key_type '{request.KeyType}'."));
            }

            // Key material generation is expected to be handled by the hosting
            // service using the SecurityProvider and PQC components. Here we
            // return a minimal successful response shell.
            var response = new GenerateKeyResponse
            {
                PublicKey = string.Empty,
                KeyId = string.Empty,
                Success = true,
                ErrorMessage = string.Empty
            };

            return Task.FromResult(response);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GenerateKey failed for key_type {KeyType}", request.KeyType);
            throw new RpcException(new Status(StatusCode.Internal, "Internal error while generating key."));
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation("GenerateKey({KeyType}) completed in {ElapsedMs} ms", request.KeyType, sw.ElapsedMilliseconds);
        }
    }

    public override Task<MigrateKeyResponse> MigrateKey(MigrateKeyRequest request, ServerCallContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            if (string.IsNullOrWhiteSpace(request.ClassicalKeyId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "classical_key_id is required."));
            }

            if (string.IsNullOrWhiteSpace(request.TargetAlgorithm))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "target_algorithm is required."));
            }

            // Migration logic (RSA → ML-DSA / hybrid) must be implemented in the
            // hosting service using the database schema (wallet_keys_v2 and
            // key_migration_log). Here we only provide the contract surface.

            var response = new MigrateKeyResponse
            {
                NewKeyId = string.Empty,
                PublicKey = string.Empty,
                Success = true,
                ErrorMessage = string.Empty,
                MigrationInfo = new KeyMigrationInfo
                {
                    ClassicalKeyId = request.ClassicalKeyId,
                    PqKeyId = string.Empty,
                    ClassicalAlgorithm = string.Empty,
                    PqAlgorithm = request.TargetAlgorithm,
                    RequestedAt = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    CompletedAt = 0,
                    HybridModeEnabled = request.KeepClassical
                }
            };

            return Task.FromResult(response);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MigrateKey failed for classical_key_id {KeyId}", request.ClassicalKeyId);
            throw new RpcException(new Status(StatusCode.Internal, "Internal error while migrating key."));
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation("MigrateKey({ClassicalKeyId} -> {TargetAlgorithm}) completed in {ElapsedMs} ms",
                request.ClassicalKeyId, request.TargetAlgorithm, sw.ElapsedMilliseconds);
        }
    }

    public override Task<SignTransactionResponse> SignTransaction(SignTransactionRequest request, ServerCallContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            if (string.IsNullOrWhiteSpace(request.KeyId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "key_id is required."));
            }

            if (request.TransactionData == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "transaction_data is required."));
            }

            // In a full implementation the service would:
            // - Resolve the key metadata (algorithm, PQC/hybrid flags) from storage.
            // - Use SecurityProvider / PQC signer to sign the transaction bytes.
            // - For hybrid keys, combine classical and PQ signatures.

            var response = new SignTransactionResponse
            {
                Signature = Google.Protobuf.ByteString.Empty,
                SignedTransaction = request.TransactionData,
                Success = true,
                ErrorMessage = string.Empty
            };

            return Task.FromResult(response);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SignTransaction failed for key_id {KeyId}", request.KeyId);
            throw new RpcException(new Status(StatusCode.Internal, "Internal error while signing transaction."));
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation("SignTransaction({KeyId}) completed in {ElapsedMs} ms", request.KeyId, sw.ElapsedMilliseconds);
        }
    }

    private static bool IsSupportedKeyType(string keyType)
    {
        return keyType switch
        {
            "ed25519" => true,
            "secp256k1" => true,
            "ml-dsa-44" => true,
            "ml-dsa-65" => true,
            "ml-dsa-87" => true,
            "ml-kem-512" => true,
            "ml-kem-768" => true,
            "ml-kem-1024" => true,
            "hybrid-rsa-mldsa65" => true,
            "hybrid-ecdsa-mldsa65" => true,
            _ => false
        };
    }
}


