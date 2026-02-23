using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;

namespace AppBuilder.Infrastructure.Services;

/// <summary>Platform plugins – Android (included), WordPress (paid). Appy: Extensible, future iOS/Windows/macOS.</summary>
public class PluginService : IPluginService
{
    private static readonly List<PluginPlatform> _platforms = new()
    {
        new PluginPlatform { Id = "android", Name = "Android", Description = "Convert any website into a native Android app. APK ready for Play Store.", IsIncluded = true, IsPaid = false, IsEnabled = true },
        new PluginPlatform { Id = "wordpress", Name = "WordPress", Description = "Create native apps for WordPress blogs. Paid plugin.", IsIncluded = false, IsPaid = true, ProductUrl = "https://codecanyon.net/wordpress-app-plugin", IsEnabled = true }
    };

    public IReadOnlyList<PluginPlatform> GetPlatforms() => _platforms.Where(p => p.IsEnabled).ToList();
    public PluginPlatform? GetPlatform(string id) => _platforms.FirstOrDefault(p => p.Id == id);
}
