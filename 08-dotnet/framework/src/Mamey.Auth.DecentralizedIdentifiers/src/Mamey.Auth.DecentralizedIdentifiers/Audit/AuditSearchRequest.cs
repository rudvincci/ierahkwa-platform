using System;
using System.Collections.Generic;

namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// Request for searching audit events
/// </summary>
public class AuditSearchRequest
{
    /// <summary>
    /// Start date for search
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// End date for search
    /// </summary>
    public DateTime? To { get; set; }

    /// <summary>
    /// DID to filter by
    /// </summary>
    public string? Did { get; set; }

    /// <summary>
    /// Event types to filter by
    /// </summary>
    public List<string>? EventTypes { get; set; }

    /// <summary>
    /// Severity levels to filter by
    /// </summary>
    public List<AuditSeverity>? Severities { get; set; }

    /// <summary>
    /// Source to filter by
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Tenant ID to filter by
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// IP address to filter by
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Session ID to filter by
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Request ID to filter by
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Search term for metadata
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Maximum number of results
    /// </summary>
    public int Limit { get; set; } = 100;

    /// <summary>
    /// Offset for pagination
    /// </summary>
    public int Offset { get; set; } = 0;

    /// <summary>
    /// Sort field
    /// </summary>
    public string SortField { get; set; } = "Timestamp";

    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;
}

/// <summary>
/// Result of audit search
/// </summary>
public class AuditSearchResult
{
    /// <summary>
    /// Audit events found
    /// </summary>
    public List<AuditEvent> Events { get; set; } = new();

    /// <summary>
    /// Total number of events matching the search criteria
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Number of events returned in this page
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Whether there are more results
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// Next offset for pagination
    /// </summary>
    public int NextOffset { get; set; }
}

/// <summary>
/// Sort direction
/// </summary>
public enum SortDirection
{
    Ascending,
    Descending
}

/// <summary>
/// Audit statistics
/// </summary>
public class AuditStatistics
{
    /// <summary>
    /// Total number of events
    /// </summary>
    public int TotalEvents { get; set; }

    /// <summary>
    /// Events by type
    /// </summary>
    public Dictionary<string, int> EventsByType { get; set; } = new();

    /// <summary>
    /// Events by severity
    /// </summary>
    public Dictionary<AuditSeverity, int> EventsBySeverity { get; set; } = new();

    /// <summary>
    /// Authentication success rate
    /// </summary>
    public double AuthenticationSuccessRate { get; set; }

    /// <summary>
    /// Average resolution time in milliseconds
    /// </summary>
    public double AverageResolutionTimeMs { get; set; }

    /// <summary>
    /// Cache hit rate for DID resolutions
    /// </summary>
    public double CacheHitRate { get; set; }

    /// <summary>
    /// Number of security events
    /// </summary>
    public int SecurityEventsCount { get; set; }

    /// <summary>
    /// Most active DIDs
    /// </summary>
    public List<DidActivity> MostActiveDids { get; set; } = new();

    /// <summary>
    /// Events by hour of day
    /// </summary>
    public Dictionary<int, int> EventsByHour { get; set; } = new();

    /// <summary>
    /// Events by day of week
    /// </summary>
    public Dictionary<DayOfWeek, int> EventsByDayOfWeek { get; set; } = new();
}

/// <summary>
/// DID activity information
/// </summary>
public class DidActivity
{
    /// <summary>
    /// DID identifier
    /// </summary>
    public string Did { get; set; } = string.Empty;

    /// <summary>
    /// Number of events
    /// </summary>
    public int EventCount { get; set; }

    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTime LastActivity { get; set; }

    /// <summary>
    /// Most common event type
    /// </summary>
    public string MostCommonEventType { get; set; } = string.Empty;
}











