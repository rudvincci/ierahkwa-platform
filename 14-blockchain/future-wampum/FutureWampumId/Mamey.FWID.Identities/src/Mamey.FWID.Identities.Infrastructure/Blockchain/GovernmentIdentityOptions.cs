namespace Mamey.FWID.Identities.Infrastructure.Blockchain;

/// <summary>
/// Configuration options for Government Identity blockchain integration.
/// </summary>
public class GovernmentIdentityOptions
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "governmentIdentity";

    /// <summary>
    /// The MameyNode gRPC endpoint URL.
    /// Default: https://localhost:50051
    /// </summary>
    public string NodeUrl { get; set; } = "https://localhost:50051";

    /// <summary>
    /// Whether government identity integration is enabled.
    /// When disabled, identities will not be registered on the blockchain.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Timeout for blockchain operations in seconds.
    /// Default: 30 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to fail identity creation if blockchain registration fails.
    /// When false, identity creation continues even if blockchain registration fails.
    /// Default: false (best effort registration).
    /// </summary>
    public bool FailOnBlockchainError { get; set; } = false;

    /// <summary>
    /// Whether to automatically register identities on the blockchain.
    /// Default: true.
    /// </summary>
    public bool AutoRegisterIdentities { get; set; } = true;

    /// <summary>
    /// Whether to verify identities against blockchain on authentication.
    /// Default: false (performance consideration).
    /// </summary>
    public bool VerifyOnAuthentication { get; set; } = false;
}
