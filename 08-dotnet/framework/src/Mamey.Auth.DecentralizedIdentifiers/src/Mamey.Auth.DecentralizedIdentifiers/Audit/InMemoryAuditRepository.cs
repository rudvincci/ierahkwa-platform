using Microsoft.Extensions.Logging;
using Mamey.Types;

namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// In-memory implementation of the audit repository for testing and development purposes.
/// </summary>
public class InMemoryAuditRepository : IAuditRepository
{
    private readonly ILogger<InMemoryAuditRepository> _logger;
    private readonly List<AuditLog> _auditLogs = new();
    private readonly object _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryAuditRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public InMemoryAuditRepository(ILogger<InMemoryAuditRepository> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task SaveAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _auditLogs.Add(auditLog);
        }

        _logger.LogDebug("Audit log saved in memory: {Id} - {ActivityType}", auditLog.Id, auditLog.ActivityType);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets all audit logs stored in memory.
    /// </summary>
    /// <returns>A list of all audit logs.</returns>
    public IReadOnlyList<AuditLog> GetAllAuditLogs()
    {
        lock (_lock)
        {
            return _auditLogs.ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// Gets audit logs by activity type.
    /// </summary>
    /// <param name="activityType">The activity type to filter by.</param>
    /// <returns>A list of audit logs matching the activity type.</returns>
    public IReadOnlyList<AuditLog> GetAuditLogsByActivityType(string activityType)
    {
        lock (_lock)
        {
            return _auditLogs
                .Where(log => log.ActivityType == activityType)
                .ToList()
                .AsReadOnly();
        }
    }

    /// <summary>
    /// Gets audit logs by category.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <returns>A list of audit logs matching the category.</returns>
    public IReadOnlyList<AuditLog> GetAuditLogsByCategory(string category)
    {
        lock (_lock)
        {
            return _auditLogs
                .Where(log => log.Category == category)
                .ToList()
                .AsReadOnly();
        }
    }

    /// <summary>
    /// Gets audit logs by correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID to filter by.</param>
    /// <returns>A list of audit logs matching the correlation ID.</returns>
    public IReadOnlyList<AuditLog> GetAuditLogsByCorrelationId(string correlationId)
    {
        lock (_lock)
        {
            return _auditLogs
                .Where(log => log.CorrelationId == correlationId)
                .ToList()
                .AsReadOnly();
        }
    }

    /// <summary>
    /// Clears all audit logs from memory.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _auditLogs.Clear();
        }
    }

    /// <summary>
    /// Gets the count of audit logs stored in memory.
    /// </summary>
    /// <returns>The count of audit logs.</returns>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _auditLogs.Count;
            }
        }
    }
}







