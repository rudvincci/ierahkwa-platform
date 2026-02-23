namespace MameyFramework.Core;

/// <summary>
/// Base interface for all Mamey services
/// </summary>
public interface IMameyService
{
    string ServiceName { get; }
    string ServiceVersion { get; }
    ServiceStatus Status { get; }
    
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default);
}

public enum ServiceStatus
{
    Stopped,
    Starting,
    Running,
    Stopping,
    Failed
}

public record HealthCheckResult(
    bool IsHealthy,
    string? Message = null,
    Dictionary<string, object>? Data = null
);

/// <summary>
/// Base implementation for Mamey services
/// </summary>
public abstract class MameyServiceBase : IMameyService
{
    public abstract string ServiceName { get; }
    public virtual string ServiceVersion => "1.0.0";
    public ServiceStatus Status { get; protected set; } = ServiceStatus.Stopped;
    
    protected readonly ILogger Logger;
    
    protected MameyServiceBase(ILogger logger)
    {
        Logger = logger;
    }
    
    public virtual Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("{Service} initializing...", ServiceName);
        return Task.CompletedTask;
    }
    
    public virtual async Task StartAsync(CancellationToken cancellationToken = default)
    {
        Status = ServiceStatus.Starting;
        Logger.LogInformation("{Service} starting...", ServiceName);
        
        await OnStartAsync(cancellationToken);
        
        Status = ServiceStatus.Running;
        Logger.LogInformation("{Service} started successfully", ServiceName);
    }
    
    public virtual async Task StopAsync(CancellationToken cancellationToken = default)
    {
        Status = ServiceStatus.Stopping;
        Logger.LogInformation("{Service} stopping...", ServiceName);
        
        await OnStopAsync(cancellationToken);
        
        Status = ServiceStatus.Stopped;
        Logger.LogInformation("{Service} stopped", ServiceName);
    }
    
    public virtual Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new HealthCheckResult(
            Status == ServiceStatus.Running,
            Status == ServiceStatus.Running ? "Service is healthy" : $"Service status: {Status}"
        ));
    }
    
    protected virtual Task OnStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    protected virtual Task OnStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

/// <summary>
/// Logger interface compatible with Microsoft.Extensions.Logging
/// </summary>
public interface ILogger
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception? exception, string message, params object[] args);
    void LogDebug(string message, params object[] args);
}
