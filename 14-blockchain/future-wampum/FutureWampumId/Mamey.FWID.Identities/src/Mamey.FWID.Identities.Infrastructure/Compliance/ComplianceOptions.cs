namespace Mamey.FWID.Identities.Infrastructure.Compliance;

/// <summary>
/// Configuration options for compliance service integration.
/// </summary>
public class ComplianceOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "compliance";

    /// <summary>
    /// The MameyNode ComplianceService gRPC endpoint URL.
    /// Default: http://localhost:50051
    /// </summary>
    public string NodeUrl { get; set; } = "http://localhost:50051";

    /// <summary>
    /// Whether compliance integration is enabled.
    /// When disabled, compliance audit logging is skipped.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Timeout for compliance operations in seconds.
    /// Default: 30 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to log all identity events to compliance audit trail.
    /// Default: true.
    /// </summary>
    public bool LogAllIdentityEvents { get; set; } = true;

    /// <summary>
    /// Whether to log KYC verification events.
    /// Default: true.
    /// </summary>
    public bool LogKycEvents { get; set; } = true;

    /// <summary>
    /// Whether to automatically check AML on identity creation.
    /// Default: true.
    /// </summary>
    public bool AutoAmlCheckOnCreation { get; set; } = true;

    /// <summary>
    /// Regulations to enforce.
    /// Default: 2025-AM01 (AML), 2025-ID01 (Identity), GOV-005 (Treaty Compliance)
    /// </summary>
    public string[] Regulations { get; set; } = new[] { "2025-AM01", "2025-ID01", "2025-DS01", "GOV-005" };
}
