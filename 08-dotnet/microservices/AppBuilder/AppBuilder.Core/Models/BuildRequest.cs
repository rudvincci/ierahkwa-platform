namespace AppBuilder.Core.Models;

/// <summary>Request to trigger a new build - IERAHKWA AppBuilder</summary>
public class BuildRequest
{
    public string AppProjectId { get; set; } = string.Empty;
    public AppPlatform Platform { get; set; }
    public string? Version { get; set; }
    public string? VersionCode { get; set; }
}
