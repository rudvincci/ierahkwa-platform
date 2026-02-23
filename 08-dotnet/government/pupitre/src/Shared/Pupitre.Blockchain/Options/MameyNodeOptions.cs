namespace Pupitre.Blockchain.Options;

/// <summary>
/// Shared configuration for connecting to MameyNode services.
/// </summary>
public sealed class MameyNodeOptions
{
    /// <summary>
    /// Base gRPC endpoint for ledger/node services (e.g. http://localhost:50051).
    /// </summary>
    public string NodeUrl { get; set; } = "http://localhost:50051";

    /// <summary>
    /// Explicit host for GovernmentService when different from NodeUrl host.
    /// </summary>
    public string GovernmentHost { get; set; } = "localhost";

    /// <summary>
    /// gRPC port for GovernmentService.
    /// </summary>
    public int GovernmentPort { get; set; } = 50051;

    /// <summary>
    /// Friendly network label (development, staging, production, etc.).
    /// </summary>
    public string Network { get; set; } = "development";

    /// <summary>
    /// Optional default nationality to apply when not provided.
    /// </summary>
    public string DefaultNationality { get; set; } = "USA";
}
