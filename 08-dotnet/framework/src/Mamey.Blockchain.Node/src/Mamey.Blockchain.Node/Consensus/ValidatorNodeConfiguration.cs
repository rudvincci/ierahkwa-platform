using Mamey.Security.PostQuantum.Models;

namespace Mamey.Blockchain.Node.Consensus;

/// <summary>
/// Configuration for a validator node with post-quantum settings.
/// </summary>
public sealed class ValidatorNodeConfiguration
{
    /// <summary>
    /// When true, the validator must enforce quantum-resistant (PQC or hybrid)
    /// signatures after the configured mandatory date/block height.
    /// </summary>
    public bool RequireQuantumResistantSignatures { get; set; }

    /// <summary>
    /// Date (in UTC) after which quantum-resistant signatures become mandatory.
    /// Before this date, the network operates in a hybrid phase.
    /// </summary>
    public DateTime QuantumResistantMandatoryDate { get; set; } = DateTime.MaxValue;

    /// <summary>
    /// Optional block height after which quantum-resistant signatures become
    /// mandatory, regardless of the current date.
    /// </summary>
    public ulong? QuantumResistantMandatoryBlockHeight { get; set; }

    /// <summary>
    /// Whether hybrid transactions (carrying both classical and PQC signatures)
    /// are accepted by the validator.
    /// </summary>
    public bool AcceptHybridTransactions { get; set; } = true;

    /// <summary>
    /// Whether classical-only transactions are accepted by the validator.
    /// This is typically true during the transition phase and false once the
    /// network has fully migrated to quantum-resistant signatures.
    /// </summary>
    public bool AcceptClassicalTransactions { get; set; } = true;

    /// <summary>
    /// Maximum number of PQC transactions allowed per block. This prevents
    /// oversized PQC-heavy blocks from overwhelming the network.
    /// </summary>
    public int MaxPQCTransactionsPerBlock { get; set; } = 1000;

    /// <summary>
    /// Maximum allowed block size in bytes. Default is 10 MB.
    /// </summary>
    public int MaxBlockSizeBytes { get; set; } = 10 * 1024 * 1024;

    /// <summary>
    /// Base validation level used before any mandatory date/height is reached.
    /// Typically <see cref="PQCValidationLevel.Hybrid"/> during migration.
    /// </summary>
    public PQCValidationLevel BaseValidationLevel { get; set; } = PQCValidationLevel.Hybrid;

    /// <summary>
    /// Computes the effective validation level based on the current time and
    /// optional block height. Once the mandatory quantum-resistant threshold is
    /// reached, the network moves to <see cref="PQCValidationLevel.QuantumResistantOnly"/>.
    /// </summary>
    public PQCValidationLevel GetEffectiveValidationLevel(DateTime utcNow, ulong currentBlockHeight)
    {
        if (!RequireQuantumResistantSignatures)
        {
            return BaseValidationLevel;
        }

        var reachedDate = utcNow >= QuantumResistantMandatoryDate;
        var reachedHeight = QuantumResistantMandatoryBlockHeight.HasValue &&
                            currentBlockHeight >= QuantumResistantMandatoryBlockHeight.Value;

        return reachedDate || reachedHeight
            ? PQCValidationLevel.QuantumResistantOnly
            : BaseValidationLevel;
    }
}

/// <summary>
/// Validation level for PQC-aware transaction and block verification.
/// </summary>
public enum PQCValidationLevel
{
    ClassicalOnly = 0,
    Hybrid = 1,
    HybridRequired = 2,
    QuantumResistantOnly = 3
}


