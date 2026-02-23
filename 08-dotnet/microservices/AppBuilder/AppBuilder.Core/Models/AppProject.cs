namespace AppBuilder.Core.Models;

/// <summary>
/// App Project Model - IERAHKWA AppBuilder
/// Inspired by Appy: transform website into native apps (Android, Windows, macOS, Linux)
/// </summary>
public class AppProject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>Website URL to wrap as native app (Enter Your URL - Appy step 1)</summary>
    public string SourceUrl { get; set; } = string.Empty;

    /// <summary>Design & branding (Customize Design - Appy step 2)</summary>
    public AppDesign Design { get; set; } = new();

    /// <summary>Target platforms: Android, Windows, macOS, Linux</summary>
    public List<AppPlatform> TargetPlatforms { get; set; } = new() { AppPlatform.Android, AppPlatform.Windows };

    /// <summary>Push notifications (Firebase) - enabled/disabled</summary>
    public bool PushNotificationsEnabled { get; set; }

    /// <summary>Firebase project ID when push is enabled</summary>
    public string? FirebaseProjectId { get; set; }

    /// <summary>Hosted pages: About, FAQ, Terms (custom HTML)</summary>
    public List<HostedPage> HostedPages { get; set; } = new();

    /// <summary>Custom CSS/JS injection for full control</summary>
    public string? CustomCss { get; set; }
    public string? CustomJavaScript { get; set; }

    /// <summary>Navigation style: WebView, Tabs, Drawer</summary>
    public NavigationStyle NavigationStyle { get; set; } = NavigationStyle.WebView;

    // IERAHKWA Integration
    public string? IgtTokenId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
}

/// <summary>Design customization - colors, icons, splash (Appy: Easy Configuration, Complete Branding Control)</summary>
public class AppDesign
{
    public string PrimaryColor { get; set; } = "#FFD700";      // IERAHKWA gold
    public string SecondaryColor { get; set; } = "#00FF41";      // neon green
    public string BackgroundColor { get; set; } = "#0a0e17";

    /// <summary>Base64 or URL for app icon (192x192, 512x512)</summary>
    public string? AppIconUrl { get; set; }
    public string? SplashImageUrl { get; set; }
    public string? SplashBackgroundColor { get; set; } = "#0a0e17";

    public string StatusBarStyle { get; set; } = "default";    // default, light, dark
    public bool DarkMode { get; set; }
}

/// <summary>Hosted page - About, FAQ, Terms (Appy: Hosted Pages)</summary>
public class HostedPage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;           // about, faq, terms
    public string HtmlContent { get; set; } = string.Empty;
    public int Order { get; set; }
}

public enum AppPlatform
{
    Android,
    Windows,
    MacOS,
    Linux
}

public enum NavigationStyle
{
    WebView,    // Single WebView (default)
    Tabs,       // Bottom/top tabs
    Drawer      // Side drawer
}
