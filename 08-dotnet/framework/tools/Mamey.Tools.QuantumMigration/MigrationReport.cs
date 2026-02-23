namespace Mamey.Tools.QuantumMigration;

/// <summary>
/// Summary report for a key migration run.
/// </summary>
public sealed class MigrationReport
{
    public int TotalKeys { get; set; }
    public int SuccessfulMigrations { get; set; }
    public int FailedMigrations { get; set; }
    public TimeSpan Duration { get; set; }
    public List<MigrationError> Errors { get; } = new();

    public override string ToString()
        => $"Total: {TotalKeys}, Successful: {SuccessfulMigrations}, Failed: {FailedMigrations}, Duration: {Duration}";
}

/// <summary>
/// Detailed error information for a single key migration failure.
/// </summary>
public sealed class MigrationError
{
    public string KeyId { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Options controlling a migration run.
/// </summary>
public sealed class MigrationOptions
{
    /// <summary>
    /// When true, classical keys are retained and linked to their PQC
    /// equivalents (hybrid mode). When false, migration produces PQC-only
    /// keys.
    /// </summary>
    public bool EnableHybridMode { get; set; }

    /// <summary>
    /// When true, transactions associated with migrated keys will be
    /// re-signed with the new PQC / hybrid keys.
    /// </summary>
    public bool MigrateTransactions { get; set; }

    /// <summary>
    /// Target PQC signature algorithm identifier (e.g. "ML-DSA-65").
    /// </summary>
    public string TargetAlgorithm { get; set; } = "ML-DSA-65";

    /// <summary>
    /// Optional path to a migration report file (JSON).
    /// </summary>
    public string? ReportPath { get; set; }
}


