namespace Mamey.Identity.Decentralized.Configuration;

/// <summary>
/// Library-wide operational options (logging, debug, telemetry, etc).
/// </summary>
public class DidLibraryOptions
{
    public bool EnableDebugLogging { get; set; } = false;
    public bool EnableAuditTrail { get; set; } = true;
    public string AuditTrailFilePath { get; set; }
    public bool EnableTelemetry { get; set; } = false;
}