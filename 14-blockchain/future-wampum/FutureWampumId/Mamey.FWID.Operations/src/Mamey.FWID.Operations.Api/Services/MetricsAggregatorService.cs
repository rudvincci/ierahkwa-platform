using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Operations.Api.Services;

/// <summary>
/// Service for aggregating metrics across all FutureWampumID services.
/// </summary>
public interface IMetricsAggregatorService
{
    /// <summary>
    /// Gets aggregated operational metrics.
    /// </summary>
    Task<OperationalMetrics> GetOperationalMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets service-specific metrics.
    /// </summary>
    Task<ServiceMetrics> GetServiceMetricsAsync(string serviceName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets historical metrics for a time range.
    /// </summary>
    Task<IReadOnlyList<MetricDataPoint>> GetHistoricalMetricsAsync(
        string metricName,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets KPI dashboard data.
    /// </summary>
    Task<KPIDashboard> GetKPIDashboardAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Metrics aggregator service implementation.
/// </summary>
public class MetricsAggregatorService : IMetricsAggregatorService
{
    private readonly ILogger<MetricsAggregatorService> _logger;
    
    // Simulated metrics - in production, fetch from Prometheus
    private readonly Random _random = new();
    
    public MetricsAggregatorService(ILogger<MetricsAggregatorService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public Task<OperationalMetrics> GetOperationalMetricsAsync(CancellationToken cancellationToken = default)
    {
        var metrics = new OperationalMetrics
        {
            TotalIdentities = 15000 + _random.Next(100),
            ActiveIdentities = 12500 + _random.Next(50),
            TotalDIDs = 18000 + _random.Next(150),
            TotalCredentials = 45000 + _random.Next(500),
            ActiveCredentials = 42000 + _random.Next(300),
            RevokedCredentials = 3000 + _random.Next(50),
            TotalZKPProofs = 125000 + _random.Next(1000),
            TotalZoneAccesses = 850000 + _random.Next(5000),
            RequestsLast24h = 250000 + _random.Next(10000),
            AverageResponseTimeMs = 45 + _random.Next(20),
            ErrorRateLast24h = 0.02 + _random.NextDouble() * 0.01,
            UpTimePercentage = 99.95 + _random.NextDouble() * 0.04,
            CollectedAt = DateTime.UtcNow
        };
        
        return Task.FromResult(metrics);
    }
    
    /// <inheritdoc />
    public Task<ServiceMetrics> GetServiceMetricsAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        var metrics = new ServiceMetrics
        {
            ServiceName = serviceName,
            RequestsPerMinute = 100 + _random.Next(50),
            AverageResponseTimeMs = 30 + _random.Next(30),
            P95ResponseTimeMs = 100 + _random.Next(50),
            P99ResponseTimeMs = 200 + _random.Next(100),
            ErrorRate = _random.NextDouble() * 0.02,
            ActiveConnections = 50 + _random.Next(30),
            MemoryUsageMb = 256 + _random.Next(128),
            CpuUsagePercent = 20 + _random.NextDouble() * 30,
            CollectedAt = DateTime.UtcNow
        };
        
        return Task.FromResult(metrics);
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<MetricDataPoint>> GetHistoricalMetricsAsync(
        string metricName,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var dataPoints = new List<MetricDataPoint>();
        var interval = TimeSpan.FromMinutes(5);
        var current = from;
        
        while (current <= to)
        {
            dataPoints.Add(new MetricDataPoint
            {
                Timestamp = current,
                MetricName = metricName,
                Value = 50 + _random.NextDouble() * 100,
                Labels = new Dictionary<string, string> { ["source"] = "prometheus" }
            });
            current = current.Add(interval);
        }
        
        return Task.FromResult<IReadOnlyList<MetricDataPoint>>(dataPoints);
    }
    
    /// <inheritdoc />
    public async Task<KPIDashboard> GetKPIDashboardAsync(CancellationToken cancellationToken = default)
    {
        var metrics = await GetOperationalMetricsAsync(cancellationToken);
        
        return new KPIDashboard
        {
            KPIs = new List<KPIMetric>
            {
                new()
                {
                    Name = "Identity Registration Rate",
                    Value = 250,
                    Unit = "per day",
                    Target = 300,
                    Status = KPIStatus.OnTrack,
                    Trend = KPITrend.Up
                },
                new()
                {
                    Name = "Credential Issuance Rate",
                    Value = 850,
                    Unit = "per day",
                    Target = 1000,
                    Status = KPIStatus.OnTrack,
                    Trend = KPITrend.Up
                },
                new()
                {
                    Name = "Average Response Time",
                    Value = metrics.AverageResponseTimeMs,
                    Unit = "ms",
                    Target = 100,
                    Status = metrics.AverageResponseTimeMs < 100 ? KPIStatus.Exceeding : KPIStatus.OnTrack,
                    Trend = KPITrend.Stable
                },
                new()
                {
                    Name = "Error Rate",
                    Value = metrics.ErrorRateLast24h * 100,
                    Unit = "%",
                    Target = 1,
                    Status = metrics.ErrorRateLast24h < 0.01 ? KPIStatus.Exceeding : KPIStatus.OnTrack,
                    Trend = KPITrend.Down
                },
                new()
                {
                    Name = "System Uptime",
                    Value = metrics.UpTimePercentage,
                    Unit = "%",
                    Target = 99.9,
                    Status = metrics.UpTimePercentage >= 99.9 ? KPIStatus.Exceeding : KPIStatus.AtRisk,
                    Trend = KPITrend.Stable
                },
                new()
                {
                    Name = "Active Users",
                    Value = metrics.ActiveIdentities,
                    Unit = "users",
                    Target = 15000,
                    Status = KPIStatus.OnTrack,
                    Trend = KPITrend.Up
                }
            },
            GeneratedAt = DateTime.UtcNow
        };
    }
}

#region Models

/// <summary>
/// Aggregated operational metrics.
/// </summary>
public class OperationalMetrics
{
    public long TotalIdentities { get; set; }
    public long ActiveIdentities { get; set; }
    public long TotalDIDs { get; set; }
    public long TotalCredentials { get; set; }
    public long ActiveCredentials { get; set; }
    public long RevokedCredentials { get; set; }
    public long TotalZKPProofs { get; set; }
    public long TotalZoneAccesses { get; set; }
    public long RequestsLast24h { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public double ErrorRateLast24h { get; set; }
    public double UpTimePercentage { get; set; }
    public DateTime CollectedAt { get; set; }
}

/// <summary>
/// Service-specific metrics.
/// </summary>
public class ServiceMetrics
{
    public string ServiceName { get; set; } = null!;
    public double RequestsPerMinute { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public double P95ResponseTimeMs { get; set; }
    public double P99ResponseTimeMs { get; set; }
    public double ErrorRate { get; set; }
    public int ActiveConnections { get; set; }
    public double MemoryUsageMb { get; set; }
    public double CpuUsagePercent { get; set; }
    public DateTime CollectedAt { get; set; }
}

/// <summary>
/// Single metric data point.
/// </summary>
public class MetricDataPoint
{
    public DateTime Timestamp { get; set; }
    public string MetricName { get; set; } = null!;
    public double Value { get; set; }
    public Dictionary<string, string> Labels { get; set; } = new();
}

/// <summary>
/// KPI dashboard data.
/// </summary>
public class KPIDashboard
{
    public List<KPIMetric> KPIs { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

/// <summary>
/// Single KPI metric.
/// </summary>
public class KPIMetric
{
    public string Name { get; set; } = null!;
    public double Value { get; set; }
    public string Unit { get; set; } = null!;
    public double Target { get; set; }
    public KPIStatus Status { get; set; }
    public KPITrend Trend { get; set; }
}

public enum KPIStatus
{
    Exceeding = 1,
    OnTrack = 2,
    AtRisk = 3,
    BelowTarget = 4
}

public enum KPITrend
{
    Up = 1,
    Down = 2,
    Stable = 3
}

#endregion
