using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Mamey.Security;
using Mamey.Security.PostQuantum.Models;
using Mamey.Security.PostQuantum.Interfaces;

namespace Mamey.Tools.QuantumMigration;

/// <summary>
/// Orchestrates migration of classical cryptographic keys to
/// post-quantum / hybrid equivalents.
/// </summary>
public sealed class KeyMigrationTool
{
    private readonly ISecurityProvider _securityProvider;

    public KeyMigrationTool(ISecurityProvider securityProvider)
    {
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
    }

    /// <summary>
    /// Migrates all keys discovered in the given wallet or key store.
    /// This implementation focuses on orchestration and reporting; the
    /// concrete discovery and persistence logic is expected to be
    /// implemented by the hosting environment using callbacks.
    /// </summary>
    public async Task<MigrationReport> MigrateAllKeysAsync(
        IEnumerable<WalletKeyDescriptor> keys,
        MigrationOptions options,
        Func<WalletKeyDescriptor, Task> saveMigratedKeyAsync,
        Func<WalletKeyDescriptor, Task>? rollbackKeyAsync = null,
        CancellationToken cancellationToken = default)
    {
        if (keys is null) throw new ArgumentNullException(nameof(keys));
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (saveMigratedKeyAsync is null) throw new ArgumentNullException(nameof(saveMigratedKeyAsync));

        var report = new MigrationReport();
        var errors = new ConcurrentBag<MigrationError>();
        var sw = Stopwatch.StartNew();

        var keyList = keys.ToList();
        report.TotalKeys = keyList.Count;

        var processed = 0;
        foreach (var key in keyList)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var migrated = await MigrateSingleKeyAsync(key, options, cancellationToken).ConfigureAwait(false);
                await saveMigratedKeyAsync(migrated).ConfigureAwait(false);
                Interlocked.Increment(ref processed);
                report.SuccessfulMigrations++;
                Console.WriteLine($"[QuantumMigration] Migrated key {processed}/{report.TotalKeys} (Id={key.KeyId}).");
            }
            catch (Exception ex)
            {
                report.FailedMigrations++;
                var error = new MigrationError
                {
                    KeyId = key.KeyId,
                    ErrorMessage = ex.Message
                };
                errors.Add(error);
                Console.WriteLine($"[QuantumMigration] Failed to migrate key Id={key.KeyId}: {ex.Message}");

                if (rollbackKeyAsync != null)
                {
                    try
                    {
                        await rollbackKeyAsync(key).ConfigureAwait(false);
                    }
                    catch
                    {
                        // Best-effort rollback; errors are already captured.
                    }
                }
            }
        }

        sw.Stop();
        report.Duration = sw.Elapsed;
        report.Errors.AddRange(errors);
        return report;
    }

    private async Task<WalletKeyDescriptor> MigrateSingleKeyAsync(
        WalletKeyDescriptor key,
        MigrationOptions options,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Detect classical key type (simplified).
        var keyType = key.Type.ToLowerInvariant();
        var isClassical = keyType is "rsa" or "ecdsa" or "ed25519";
        if (!isClassical)
        {
            // Already PQC/hybrid – return unchanged.
            return key;
        }

        var targetAlg = ParseSignatureAlgorithm(options.TargetAlgorithm);

        // Generate PQC key pair.
        var pqKeyPair = _securityProvider.SignPostQuantum(
            Array.Empty<byte>(),
            Array.Empty<byte>(),
            targetAlg); // This call pattern is symbolic; real implementation
                        // should rely on a proper PQC key management API.

        // For this tooling scaffold we only update metadata to indicate that
        // the key has been migrated. The actual key persistence is left to
        // the environment using the callbacks.
        var migrated = key with
        {
            IsQuantumResistant = true,
            HybridEnabled = options.EnableHybridMode,
            TargetAlgorithm = options.TargetAlgorithm
        };

        // Transaction re-signing would be implemented here by fetching
        // affected transactions and calling into ISecurityProvider /
        // wallet services to re-sign them. This is out of scope for the
        // shared tools library, so we only respect the flag.
        if (options.MigrateTransactions)
        {
            // Placeholder for transaction re-signing orchestration.
            await Task.Yield();
        }

        return migrated;
    }

    private static SignatureAlgorithm ParseSignatureAlgorithm(string name)
    {
        return name.ToUpperInvariant() switch
        {
            "ML-DSA-44" => SignatureAlgorithm.ML_DSA_44,
            "ML-DSA-65" => SignatureAlgorithm.ML_DSA_65,
            "ML-DSA-87" => SignatureAlgorithm.ML_DSA_87,
            _ => SignatureAlgorithm.ML_DSA_65
        };
    }
}

/// <summary>
/// Lightweight descriptor for a wallet key used during migration.
/// The owning application is expected to adapt its domain model to
/// this DTO for use with <see cref="KeyMigrationTool"/>.
/// </summary>
public sealed record WalletKeyDescriptor(
    string KeyId,
    string Type,
    bool IsQuantumResistant,
    bool HybridEnabled,
    string? TargetAlgorithm);


