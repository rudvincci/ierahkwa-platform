using Microsoft.AspNetCore.Mvc;

namespace NET10.API.Controllers;

/// <summary>
/// Audit Controller - System activity logging and compliance tracking
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuditController : ControllerBase
{
    private static readonly List<AuditLog> _auditLogs = InitializeAuditLogs();
    
    /// <summary>
    /// Get audit logs with filtering
    /// </summary>
    [HttpGet]
    public ActionResult<AuditResponse> GetLogs(
        [FromQuery] string? module = null,
        [FromQuery] string? action = null,
        [FromQuery] string? userId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _auditLogs.AsQueryable();
        
        if (!string.IsNullOrEmpty(module))
            query = query.Where(l => l.Module.Equals(module, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrEmpty(action))
            query = query.Where(l => l.Action.Contains(action, StringComparison.OrdinalIgnoreCase));
        
        if (!string.IsNullOrEmpty(userId))
            query = query.Where(l => l.UserId == userId);
        
        if (fromDate.HasValue)
            query = query.Where(l => l.Timestamp >= fromDate.Value);
        
        if (toDate.HasValue)
            query = query.Where(l => l.Timestamp <= toDate.Value);
        
        var total = query.Count();
        var logs = query
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return Ok(new AuditResponse
        {
            Logs = logs,
            Total = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }
    
    /// <summary>
    /// Get audit log by ID
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<AuditLog> GetById(Guid id)
    {
        var log = _auditLogs.FirstOrDefault(l => l.Id == id);
        if (log == null) return NotFound();
        return Ok(log);
    }
    
    /// <summary>
    /// Create audit log entry
    /// </summary>
    [HttpPost]
    public ActionResult<AuditLog> CreateLog([FromBody] CreateAuditLogRequest request)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            Module = request.Module,
            Action = request.Action,
            UserId = request.UserId,
            UserName = request.UserName,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            OldValue = request.OldValue,
            NewValue = request.NewValue,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            UserAgent = Request.Headers.UserAgent.ToString(),
            Timestamp = DateTime.UtcNow,
            Severity = request.Severity,
            Notes = request.Notes
        };
        
        _auditLogs.Insert(0, log);
        
        // Keep only last 10000 logs
        if (_auditLogs.Count > 10000)
            _auditLogs.RemoveRange(10000, _auditLogs.Count - 10000);
        
        return Ok(log);
    }
    
    /// <summary>
    /// Get audit statistics
    /// </summary>
    [HttpGet("stats")]
    public ActionResult<AuditStats> GetStats([FromQuery] int days = 7)
    {
        var fromDate = DateTime.UtcNow.AddDays(-days);
        var recentLogs = _auditLogs.Where(l => l.Timestamp >= fromDate).ToList();
        
        return Ok(new AuditStats
        {
            TotalLogs = recentLogs.Count,
            LogsByModule = recentLogs
                .GroupBy(l => l.Module)
                .ToDictionary(g => g.Key, g => g.Count()),
            LogsBySeverity = recentLogs
                .GroupBy(l => l.Severity)
                .ToDictionary(g => g.Key.ToString(), g => g.Count()),
            LogsByDay = recentLogs
                .GroupBy(l => l.Timestamp.Date)
                .OrderBy(g => g.Key)
                .Select(g => new DailyLogCount { Date = g.Key, Count = g.Count() })
                .ToList(),
            TopUsers = recentLogs
                .Where(l => !string.IsNullOrEmpty(l.UserId))
                .GroupBy(l => l.UserId)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new UserLogCount { UserId = g.Key, Count = g.Count() })
                .ToList()
        });
    }
    
    /// <summary>
    /// Export audit logs to CSV
    /// </summary>
    [HttpGet("export")]
    public ActionResult ExportLogs(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var query = _auditLogs.AsQueryable();
        
        if (fromDate.HasValue)
            query = query.Where(l => l.Timestamp >= fromDate.Value);
        
        if (toDate.HasValue)
            query = query.Where(l => l.Timestamp <= toDate.Value);
        
        var logs = query.OrderByDescending(l => l.Timestamp).ToList();
        
        var csv = "Id,Timestamp,Module,Action,UserId,UserName,EntityType,EntityId,Severity,IpAddress,Notes\n";
        csv += string.Join("\n", logs.Select(l => 
            $"{l.Id},{l.Timestamp:yyyy-MM-dd HH:mm:ss},{l.Module},{l.Action},{l.UserId},{l.UserName},{l.EntityType},{l.EntityId},{l.Severity},{l.IpAddress},\"{l.Notes?.Replace("\"", "\"\"")}\""
        ));
        
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", $"audit-logs-{DateTime.UtcNow:yyyyMMdd}.csv");
    }
    
    private static List<AuditLog> InitializeAuditLogs()
    {
        var logs = new List<AuditLog>();
        var random = new Random(42);
        var modules = new[] { "ERP", "DeFi", "Swap", "Pool", "Farm", "Invoice", "Customer", "Product", "Geocoder", "Auth" };
        var actions = new[] { "Create", "Update", "Delete", "View", "Export", "Import", "Login", "Logout", "Swap", "AddLiquidity" };
        var users = new[] { "admin", "user1", "user2", "system", "api-client" };
        
        for (int i = 0; i < 500; i++)
        {
            var module = modules[random.Next(modules.Length)];
            var action = actions[random.Next(actions.Length)];
            
            logs.Add(new AuditLog
            {
                Module = module,
                Action = action,
                UserId = users[random.Next(users.Length)],
                UserName = $"User {random.Next(1, 100)}",
                EntityType = GetEntityType(module),
                EntityId = Guid.NewGuid().ToString()[..8],
                IpAddress = $"192.168.1.{random.Next(1, 255)}",
                UserAgent = "Mozilla/5.0",
                Timestamp = DateTime.UtcNow.AddMinutes(-random.Next(1, 10000)),
                Severity = (AuditSeverity)random.Next(0, 4),
                Notes = $"Auto-generated audit log #{i + 1}"
            });
        }
        
        return logs.OrderByDescending(l => l.Timestamp).ToList();
    }
    
    private static string GetEntityType(string module)
    {
        return module switch
        {
            "ERP" => "Company",
            "Invoice" => "Invoice",
            "Customer" => "Customer",
            "Product" => "Product",
            "DeFi" => "Transaction",
            "Swap" => "Swap",
            "Pool" => "Pool",
            "Farm" => "Farm",
            "Geocoder" => "GeocodingRequest",
            "Auth" => "User",
            _ => "Unknown"
        };
    }
}

// ═══════════════════════════════════════════════════════════════
// AUDIT MODELS
// ═══════════════════════════════════════════════════════════════

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public AuditSeverity Severity { get; set; } = AuditSeverity.Info;
    public string? Notes { get; set; }
}

public enum AuditSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public class AuditResponse
{
    public List<AuditLog> Logs { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class CreateAuditLogRequest
{
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public AuditSeverity Severity { get; set; } = AuditSeverity.Info;
    public string? Notes { get; set; }
}

public class AuditStats
{
    public int TotalLogs { get; set; }
    public Dictionary<string, int> LogsByModule { get; set; } = new();
    public Dictionary<string, int> LogsBySeverity { get; set; } = new();
    public List<DailyLogCount> LogsByDay { get; set; } = new();
    public List<UserLogCount> TopUsers { get; set; } = new();
}

public class DailyLogCount
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class UserLogCount
{
    public string UserId { get; set; } = string.Empty;
    public int Count { get; set; }
}
