using AppBuilder.Core.Models;

namespace AppBuilder.Core.Interfaces;

/// <summary>Platform plugins - Android (included), WordPress (paid), future iOS, Windows, macOS. Appy: Extensible Platform.</summary>
public interface IPluginService
{
    IReadOnlyList<PluginPlatform> GetPlatforms();
    PluginPlatform? GetPlatform(string id);
}
