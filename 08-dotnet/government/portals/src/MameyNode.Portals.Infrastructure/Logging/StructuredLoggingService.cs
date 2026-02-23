using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MameyNode.Portals.Infrastructure.Logging;

/// <summary>
/// Structured logging service
/// </summary>
public interface IStructuredLoggingService
{
    /// <summary>
    /// Log information with structured data
    /// </summary>
    void LogInformation(string message, Dictionary<string, object>? properties = null);
    
    /// <summary>
    /// Log warning with structured data
    /// </summary>
    void LogWarning(string message, Dictionary<string, object>? properties = null);
    
    /// <summary>
    /// Log error with structured data
    /// </summary>
    void LogError(Exception exception, string message, Dictionary<string, object>? properties = null);
    
    /// <summary>
    /// Log debug with structured data
    /// </summary>
    void LogDebug(string message, Dictionary<string, object>? properties = null);
}

/// <summary>
/// Structured logging service implementation
/// </summary>
public class StructuredLoggingService : IStructuredLoggingService
{
    private readonly ILogger<StructuredLoggingService> _logger;

    public StructuredLoggingService(ILogger<StructuredLoggingService> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, Dictionary<string, object>? properties = null)
    {
        if (properties != null && properties.Count > 0)
        {
            _logger.LogInformation("{Message} | Properties: {Properties}", 
                message, JsonSerializer.Serialize(properties));
        }
        else
        {
            _logger.LogInformation(message);
        }
    }

    public void LogWarning(string message, Dictionary<string, object>? properties = null)
    {
        if (properties != null && properties.Count > 0)
        {
            _logger.LogWarning("{Message} | Properties: {Properties}", 
                message, JsonSerializer.Serialize(properties));
        }
        else
        {
            _logger.LogWarning(message);
        }
    }

    public void LogError(Exception exception, string message, Dictionary<string, object>? properties = null)
    {
        if (properties != null && properties.Count > 0)
        {
            _logger.LogError(exception, "{Message} | Properties: {Properties}", 
                message, JsonSerializer.Serialize(properties));
        }
        else
        {
            _logger.LogError(exception, message);
        }
    }

    public void LogDebug(string message, Dictionary<string, object>? properties = null)
    {
        if (properties != null && properties.Count > 0)
        {
            _logger.LogDebug("{Message} | Properties: {Properties}", 
                message, JsonSerializer.Serialize(properties));
        }
        else
        {
            _logger.LogDebug(message);
        }
    }
}


