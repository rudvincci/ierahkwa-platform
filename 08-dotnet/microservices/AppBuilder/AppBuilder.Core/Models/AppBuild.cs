namespace AppBuilder.Core.Models;

/// <summary>
/// App Build Record - IERAHKWA AppBuilder
/// Build tracking, version management (Appy: Build Tracking, Version Management, QR Code Testing)
/// </summary>
public class AppBuild
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AppProjectId { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public string? VersionCode { get; set; }                   // Android versionCode, etc.

    public AppPlatform Platform { get; set; }
    public BuildStatus Status { get; set; } = BuildStatus.Pending;

    /// <summary>Build start/end for duration tracking</summary>
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? DurationSeconds { get; set; }

    /// <summary>Download URL when build succeeds; QR code data for testing</summary>
    public string? DownloadUrl { get; set; }
    public string? QrCodeDataUrl { get; set; }

    /// <summary>Error message when failed</summary>
    public string? ErrorMessage { get; set; }
    public string? BuildLog { get; set; }

    /// <summary>Appy: App Signing - ready for store distribution</summary>
    public bool Signed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum BuildStatus
{
    Pending,
    Building,
    Success,
    Failed,
    Cancelled
}
