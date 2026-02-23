namespace Mamey.AI.Government.Models;

public class FraudIndicator
{
    public string Type { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Evidence { get; set; } = new();
}

public enum FraudIndicatorType
{
    DuplicateApplication,
    IdentityMismatch,
    DocumentTampering,
    SuspiciousPattern,
    VelocityAbuse,
    DeviceFingerprint,
    IPGeolocation,
    BehavioralAnomaly
}
