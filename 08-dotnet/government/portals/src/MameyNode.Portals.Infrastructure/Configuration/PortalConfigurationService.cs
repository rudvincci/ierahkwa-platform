using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MameyNode.Portals.Infrastructure.Configuration;

/// <summary>
/// Portal configuration service
/// </summary>
public interface IPortalConfigurationService
{
    /// <summary>
    /// Get portal settings
    /// </summary>
    PortalSettings? GetPortalSettings(string portalName);
    
    /// <summary>
    /// Get all portal settings
    /// </summary>
    Dictionary<string, PortalSettings> GetAllPortalSettings();
    
    /// <summary>
    /// Get configuration value
    /// </summary>
    T? GetValue<T>(string key);
    
    /// <summary>
    /// Get configuration section
    /// </summary>
    IConfigurationSection GetSection(string key);
}

/// <summary>
/// Portal configuration service implementation
/// </summary>
public class PortalConfigurationService : IPortalConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly IOptions<Dictionary<string, PortalSettings>> _portalSettings;

    public PortalConfigurationService(
        IConfiguration configuration,
        IOptions<Dictionary<string, PortalSettings>> portalSettings)
    {
        _configuration = configuration;
        _portalSettings = portalSettings;
    }

    public PortalSettings? GetPortalSettings(string portalName)
    {
        if (_portalSettings.Value?.TryGetValue(portalName, out var settings) == true)
        {
            return settings;
        }
        return null;
    }

    public Dictionary<string, PortalSettings> GetAllPortalSettings()
    {
        return _portalSettings.Value ?? new Dictionary<string, PortalSettings>();
    }

    public T? GetValue<T>(string key)
    {
        return _configuration.GetValue<T>(key);
    }

    public IConfigurationSection GetSection(string key)
    {
        return _configuration.GetSection(key);
    }
}

/// <summary>
/// Portal settings model
/// </summary>
public class PortalSettings
{
    public string[] AuthMethods { get; set; } = Array.Empty<string>();
    public string Policy { get; set; } = "EitherOr";
    public string[] Routes { get; set; } = Array.Empty<string>();
    public Dictionary<string, string>? AdditionalSettings { get; set; }
}


