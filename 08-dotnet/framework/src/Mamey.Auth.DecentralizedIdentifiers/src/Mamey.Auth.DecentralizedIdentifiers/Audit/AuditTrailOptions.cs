using System;

namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// Configuration options for audit trail
/// </summary>
public class AuditTrailOptions
{
    public bool Enabled { get; set; } = true;
    public int SuspiciousActivityThreshold { get; set; } = 5;
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(90);
    public bool EnableRealTimeAlerts { get; set; } = true;
    public string[] SensitiveFields { get; set; } = Array.Empty<string>();
}
