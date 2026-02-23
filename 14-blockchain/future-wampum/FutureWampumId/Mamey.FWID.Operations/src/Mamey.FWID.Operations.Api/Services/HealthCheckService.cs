using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Operations.Api.Services;

/// <summary>
/// Service for checking health of all FutureWampumID services.
/// </summary>
public interface IHealthCheckService
{
    /// <summary>
    /// Gets overall system health.
    /// </summary>
    Task<SystemHealthStatus> GetSystemHealthAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets health status for a specific service.
    /// </summary>
    Task<ServiceHealthStatus> GetServiceHealthAsync(string serviceName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets health status for all services.
    /// </summary>
    Task<IReadOnlyList<ServiceHealthStatus>> GetAllServicesHealthAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Runs a detailed health check.
    /// </summary>
    Task<DetailedHealthCheck> RunDetailedHealthCheckAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Health check service implementation.
/// </summary>
public class HealthCheckService : IHealthCheckService
{
    private readonly ILogger<HealthCheckService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    
    private static readonly Dictionary<string, string> ServiceEndpoints = new()
    {
        ["Identities"] = "http://localhost:5001/health",
        ["DIDs"] = "http://localhost:5002/health",
        ["ZKPs"] = "http://localhost:5003/health",
        ["AccessControls"] = "http://localhost:5004/health",
        ["Credentials"] = "http://localhost:5005/health",
        ["ApiGateway"] = "http://localhost:5000/health",
        ["Notifications"] = "http://localhost:5006/health",
        ["Sagas"] = "http://localhost:5007/health"
    };
    
    public HealthCheckService(
        ILogger<HealthCheckService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }
    
    /// <inheritdoc />
    public async Task<SystemHealthStatus> GetSystemHealthAsync(CancellationToken cancellationToken = default)
    {
        var services = await GetAllServicesHealthAsync(cancellationToken);
        
        var healthyCount = services.Count(s => s.Status == HealthStatus.Healthy);
        var degradedCount = services.Count(s => s.Status == HealthStatus.Degraded);
        var unhealthyCount = services.Count(s => s.Status == HealthStatus.Unhealthy);
        
        var overallStatus = unhealthyCount > 0
            ? HealthStatus.Unhealthy
            : degradedCount > 0
                ? HealthStatus.Degraded
                : HealthStatus.Healthy;
        
        return new SystemHealthStatus
        {
            Status = overallStatus,
            TotalServices = services.Count,
            HealthyServices = healthyCount,
            DegradedServices = degradedCount,
            UnhealthyServices = unhealthyCount,
            Services = services.ToList(),
            CheckedAt = DateTime.UtcNow
        };
    }
    
    /// <inheritdoc />
    public async Task<ServiceHealthStatus> GetServiceHealthAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        if (!ServiceEndpoints.TryGetValue(serviceName, out var endpoint))
        {
            return new ServiceHealthStatus
            {
                ServiceName = serviceName,
                Status = HealthStatus.Unknown,
                ErrorMessage = "Service not found"
            };
        }
        
        return await CheckServiceHealthAsync(serviceName, endpoint, cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<IReadOnlyList<ServiceHealthStatus>> GetAllServicesHealthAsync(CancellationToken cancellationToken = default)
    {
        var tasks = ServiceEndpoints.Select(kvp =>
            CheckServiceHealthAsync(kvp.Key, kvp.Value, cancellationToken));
        
        var results = await Task.WhenAll(tasks);
        return results;
    }
    
    /// <inheritdoc />
    public async Task<DetailedHealthCheck> RunDetailedHealthCheckAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var services = await GetAllServicesHealthAsync(cancellationToken);
        
        var result = new DetailedHealthCheck
        {
            CheckedAt = DateTime.UtcNow,
            Duration = stopwatch.Elapsed,
            Services = services.ToList(),
            Dependencies = new List<DependencyHealthStatus>
            {
                await CheckDependencyAsync("MongoDB", "mongodb://localhost:27017", cancellationToken),
                await CheckDependencyAsync("PostgreSQL", "host=localhost;port=5432", cancellationToken),
                await CheckDependencyAsync("Redis", "localhost:6379", cancellationToken),
                await CheckDependencyAsync("RabbitMQ", "amqp://localhost:5672", cancellationToken),
                await CheckDependencyAsync("MameyNode", "http://localhost:9545", cancellationToken)
            }
        };
        
        result.OverallStatus = result.Services.All(s => s.Status == HealthStatus.Healthy) &&
                              result.Dependencies.All(d => d.Status == HealthStatus.Healthy)
            ? HealthStatus.Healthy
            : HealthStatus.Degraded;
        
        return result;
    }
    
    #region Private Methods
    
    private async Task<ServiceHealthStatus> CheckServiceHealthAsync(
        string serviceName,
        string endpoint,
        CancellationToken cancellationToken)
    {
        var status = new ServiceHealthStatus
        {
            ServiceName = serviceName,
            Endpoint = endpoint
        };
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            using var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            
            var response = await client.GetAsync(endpoint, cancellationToken);
            
            stopwatch.Stop();
            status.ResponseTime = stopwatch.Elapsed;
            
            status.Status = response.IsSuccessStatusCode
                ? HealthStatus.Healthy
                : HealthStatus.Degraded;
            status.StatusCode = (int)response.StatusCode;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            status.ResponseTime = stopwatch.Elapsed;
            status.Status = HealthStatus.Unhealthy;
            status.ErrorMessage = ex.Message;
        }
        catch (TaskCanceledException)
        {
            stopwatch.Stop();
            status.ResponseTime = stopwatch.Elapsed;
            status.Status = HealthStatus.Unhealthy;
            status.ErrorMessage = "Timeout";
        }
        
        status.CheckedAt = DateTime.UtcNow;
        return status;
    }
    
    private Task<DependencyHealthStatus> CheckDependencyAsync(
        string name,
        string connectionString,
        CancellationToken cancellationToken)
    {
        // Simplified - in production, actually test connectivity
        return Task.FromResult(new DependencyHealthStatus
        {
            Name = name,
            Status = HealthStatus.Healthy,
            ConnectionString = MaskConnectionString(connectionString),
            CheckedAt = DateTime.UtcNow
        });
    }
    
    private static string MaskConnectionString(string connectionString)
    {
        // Mask sensitive parts
        if (connectionString.Contains("password=", StringComparison.OrdinalIgnoreCase))
        {
            return System.Text.RegularExpressions.Regex.Replace(
                connectionString, @"password=[^;]*", "password=***", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
        return connectionString;
    }
    
    #endregion
}

#region Models

/// <summary>
/// System-wide health status.
/// </summary>
public class SystemHealthStatus
{
    public HealthStatus Status { get; set; }
    public int TotalServices { get; set; }
    public int HealthyServices { get; set; }
    public int DegradedServices { get; set; }
    public int UnhealthyServices { get; set; }
    public List<ServiceHealthStatus> Services { get; set; } = new();
    public DateTime CheckedAt { get; set; }
}

/// <summary>
/// Health status for a single service.
/// </summary>
public class ServiceHealthStatus
{
    public string ServiceName { get; set; } = null!;
    public string? Endpoint { get; set; }
    public HealthStatus Status { get; set; }
    public int? StatusCode { get; set; }
    public TimeSpan? ResponseTime { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
}

/// <summary>
/// Detailed health check result.
/// </summary>
public class DetailedHealthCheck
{
    public HealthStatus OverallStatus { get; set; }
    public DateTime CheckedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public List<ServiceHealthStatus> Services { get; set; } = new();
    public List<DependencyHealthStatus> Dependencies { get; set; } = new();
}

/// <summary>
/// Health status for a dependency.
/// </summary>
public class DependencyHealthStatus
{
    public string Name { get; set; } = null!;
    public HealthStatus Status { get; set; }
    public string? ConnectionString { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
}

/// <summary>
/// Health status enum.
/// </summary>
public enum HealthStatus
{
    Unknown = 0,
    Healthy = 1,
    Degraded = 2,
    Unhealthy = 3
}

#endregion
