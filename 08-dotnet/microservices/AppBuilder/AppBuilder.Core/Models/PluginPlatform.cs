namespace AppBuilder.Core.Models;

/// <summary>Plugin / Platform - Appy: Android (included), WordPress (paid), future iOS, Windows, macOS.</summary>
public class PluginPlatform
{
    public string Id { get; set; } = "android";
    public string Name { get; set; } = "Android";
    public string Description { get; set; } = "Convert any website into a native Android app.";
    public bool IsIncluded { get; set; } = true;
    public bool IsPaid { get; set; }
    public string? ProductUrl { get; set; }
    public bool IsEnabled { get; set; } = true;
}
