using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Models;

namespace Mamey.Blockchain.Node.Consensus;

/// <summary>
/// Abstraction over classical and post-quantum signature verification.
/// Implementations are expected to delegate to the appropriate crypto
/// primitives (e.g. Mamey.Security / Mamey.Security.PostQuantum or
/// a dedicated crypto microservice).
/// </summary>
public interface IBlockSignatureVerifier
{
    bool VerifyClassical(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> publicKey);

    bool VerifyPostQuantum(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> publicKey,
        SignatureAlgorithm algorithm);
}

/// <summary>
/// Result of validating a single transaction under a specific PQC policy.
/// </summary>
public sealed class PQCTransactionValidationResult
{
    public bool IsValid { get; init; }
    public bool ClassicalValid { get; init; }
    public bool PostQuantumValid { get; init; }
    public string? Reason { get; init; }
}

/// <summary>
/// Validator that applies PQC-aware consensus rules to transactions and blocks.
/// </summary>
public sealed class PQCValidator
{
    private readonly IBlockSignatureVerifier _signatureVerifier;

    public PQCValidator(IBlockSignatureVerifier signatureVerifier)
    {
        _signatureVerifier = signatureVerifier ?? throw new ArgumentNullException(nameof(signatureVerifier));
    }

    /// <summary>
    /// Validates a single transaction under the provided configuration and
    /// returns a rich result describing classical/PQC validity.
    /// </summary>
    public PQCTransactionValidationResult ValidateTransaction(
        MameyTransaction transaction,
        ValidatorNodeConfiguration configuration,
        ReadOnlySpan<byte> publicKey,
        DateTime utcNow,
        ulong currentBlockHeight)
    {
        if (transaction is null) throw new ArgumentNullException(nameof(transaction));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var level = configuration.GetEffectiveValidationLevel(utcNow, currentBlockHeight);
        var isHybrid = transaction.IsHybrid;
        var isQuantumResistant = transaction.IsQuantumResistant;
        var isClassicalOnly = !isHybrid && !isQuantumResistant;
        var isPqcOnly = !isHybrid && isQuantumResistant;

        bool classicalValid = false;
        bool pqValid = false;
        string? reason = null;

        // Copy ref-like type to array for use in local functions
        var publicKeyArray = publicKey.ToArray();

        // Helper to run classical verification if we have a classical signature.
        bool VerifyClassical()
        {
            var sig = transaction.ClassicalSignature ?? transaction.Signature;
            if (sig is null || sig.Length == 0) return false;
            return _signatureVerifier.VerifyClassical(transaction.Data, sig, publicKeyArray);
        }

        // Helper to run PQC verification if we have a PQC signature.
        bool VerifyPqc()
        {
            var sig = transaction.PQSignature ?? transaction.Signature;
            if (sig is null || sig.Length == 0) return false;
            return _signatureVerifier.VerifyPostQuantum(transaction.Data, sig, publicKeyArray, transaction.SignatureAlgorithm);
        }

        switch (level)
        {
            case PQCValidationLevel.ClassicalOnly:
            {
                if (!configuration.AcceptClassicalTransactions)
                {
                    reason = "Classical transactions are disabled by configuration.";
                    break;
                }

                if (!isClassicalOnly)
                {
                    reason = "PQC or hybrid transaction rejected in ClassicalOnly mode.";
                    break;
                }

                classicalValid = VerifyClassical();
                if (!classicalValid)
                {
                    reason = "Classical signature verification failed.";
                }
                break;
            }

            case PQCValidationLevel.Hybrid:
            {
                // Hybrid mode accepts both classical-only and PQC/hybrid
                // transactions. Classical-only transactions are validated
                // using classical crypto; PQC or hybrid transactions are
                // validated using PQC crypto.
                if (isClassicalOnly)
                {
                    if (!configuration.AcceptClassicalTransactions)
                    {
                        reason = "Classical transactions are disabled by configuration.";
                        break;
                    }

                    classicalValid = VerifyClassical();
                    if (!classicalValid)
                    {
                        reason = "Classical signature verification failed.";
                    }
                }
                else
                {
                    if (!configuration.AcceptHybridTransactions && isHybrid)
                    {
                        reason = "Hybrid transactions are disabled by configuration.";
                        break;
                    }

                    pqValid = VerifyPqc();
                    if (!pqValid)
                    {
                        reason = "Post-quantum signature verification failed.";
                    }
                }

                break;
            }

            case PQCValidationLevel.HybridRequired:
            {
                // HybridRequired mode mandates that both classical and PQC
                // signatures are present and valid.
                if (!isHybrid)
                {
                    reason = "HybridRequired mode requires hybrid transactions.";
                    break;
                }

                if (!configuration.AcceptHybridTransactions)
                {
                    reason = "Hybrid transactions are disabled by configuration.";
                    break;
                }

                if (transaction.ClassicalSignature is null || transaction.ClassicalSignature.Length == 0 ||
                    transaction.PQSignature is null || transaction.PQSignature.Length == 0)
                {
                    reason = "Hybrid transaction is missing classical or PQC signature component.";
                    break;
                }

                classicalValid = VerifyClassical();
                pqValid = VerifyPqc();

                if (!classicalValid || !pqValid)
                {
                    reason = !classicalValid
                        ? "HybridRequired: classical signature verification failed."
                        : "HybridRequired: post-quantum signature verification failed.";
                }

                break;
            }

            case PQCValidationLevel.QuantumResistantOnly:
            {
                // QuantumResistantOnly mode rejects classical-only transactions
                // and requires PQC (or hybrid) signatures to be valid.
                if (!isQuantumResistant)
                {
                    reason = "Classical-only transaction rejected in QuantumResistantOnly mode.";
                    break;
                }

                pqValid = VerifyPqc();
                if (!pqValid)
                {
                    reason = "Post-quantum signature verification failed.";
                }

                break;
            }

            default:
                reason = "Unknown validation level.";
                break;
        }

        var isValid = level switch
        {
            PQCValidationLevel.ClassicalOnly => classicalValid,
            PQCValidationLevel.Hybrid => isClassicalOnly ? classicalValid : pqValid,
            PQCValidationLevel.HybridRequired => classicalValid && pqValid,
            PQCValidationLevel.QuantumResistantOnly => pqValid,
            _ => false
        };

        return new PQCTransactionValidationResult
        {
            IsValid = isValid,
            ClassicalValid = classicalValid,
            PostQuantumValid = pqValid,
            Reason = isValid ? null : reason
        };
    }

    /// <summary>
    /// Validates the aggregate block size against configuration limits.
    /// </summary>
    public bool ValidateBlockSize(IReadOnlyList<MameyTransaction> transactions,
        ValidatorNodeConfiguration configuration,
        out int totalSizeBytes)
    {
        if (transactions is null) throw new ArgumentNullException(nameof(transactions));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var total = 0;
        foreach (var tx in transactions)
        {
            if (tx is null) continue;
            total += (tx.Data?.Length ?? 0) + (tx.Signature?.Length ?? 0);
        }

        totalSizeBytes = total;
        return total <= configuration.MaxBlockSizeBytes;
    }

    /// <summary>
    /// Orders a transaction pool such that, after the migration deadline, PQC
    /// transactions are prioritised over classical-only transactions while
    /// preserving relative ordering within each group.
    /// </summary>
    public IReadOnlyList<MameyTransaction> OrderTransactionPool(
        IEnumerable<MameyTransaction> pool,
        ValidatorNodeConfiguration configuration,
        DateTime utcNow)
    {
        if (pool is null) throw new ArgumentNullException(nameof(pool));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var list = pool.ToList();

        if (!configuration.RequireQuantumResistantSignatures ||
            utcNow < configuration.QuantumResistantMandatoryDate)
        {
            // Before the migration deadline, preserve original order.
            return list;
        }

        var pqc = new List<MameyTransaction>();
        var classical = new List<MameyTransaction>();

        foreach (var tx in list)
        {
            if (tx is null)
            {
                continue;
            }

            if (tx.IsQuantumResistant)
            {
                pqc.Add(tx);
            }
            else
            {
                classical.Add(tx);
            }
        }

        // Enforce MaxPQCTransactionsPerBlock when prioritising PQC transactions.
        if (pqc.Count > configuration.MaxPQCTransactionsPerBlock)
        {
            pqc = pqc.Take(configuration.MaxPQCTransactionsPerBlock).ToList();
        }

        return pqc.Concat(classical).ToList();
    }
}


