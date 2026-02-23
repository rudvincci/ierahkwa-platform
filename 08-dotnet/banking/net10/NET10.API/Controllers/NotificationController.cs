using Microsoft.AspNetCore.Mvc;

namespace NET10.API.Controllers;

/// <summary>
/// Notification Controller - System alerts and user notifications
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NotificationController : ControllerBase
{
    private static readonly List<Notification> _notifications = InitializeNotifications();
    private static readonly List<SystemAlert> _alerts = InitializeAlerts();
    
    /// <summary>
    /// Get all notifications for a user
    /// </summary>
    [HttpGet("{userId}")]
    public ActionResult<NotificationResponse> GetNotifications(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userNotifications = _notifications
            .Where(n => n.UserId == userId || n.UserId == "all")
            .OrderByDescending(n => n.CreatedAt)
            .ToList();
        
        var total = userNotifications.Count;
        var items = userNotifications
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return Ok(new NotificationResponse
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
            UnreadCount = userNotifications.Count(n => !n.IsRead)
        });
    }
    
    /// <summary>
    /// Get unread count
    /// </summary>
    [HttpGet("{userId}/unread")]
    public ActionResult<int> GetUnreadCount(string userId)
    {
        var count = _notifications.Count(n => (n.UserId == userId || n.UserId == "all") && !n.IsRead);
        return Ok(new { unreadCount = count });
    }
    
    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPost("{id}/read")]
    public ActionResult MarkAsRead(Guid id)
    {
        var notification = _notifications.FirstOrDefault(n => n.Id == id);
        if (notification == null) return NotFound();
        
        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        return Ok();
    }
    
    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost("{userId}/read-all")]
    public ActionResult MarkAllAsRead(string userId)
    {
        var userNotifications = _notifications.Where(n => n.UserId == userId || n.UserId == "all");
        foreach (var n in userNotifications)
        {
            n.IsRead = true;
            n.ReadAt = DateTime.UtcNow;
        }
        return Ok();
    }
    
    /// <summary>
    /// Create a new notification
    /// </summary>
    [HttpPost]
    public ActionResult<Notification> CreateNotification([FromBody] CreateNotificationRequest request)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            Category = request.Category,
            Priority = request.Priority,
            ActionUrl = request.ActionUrl,
            CreatedAt = DateTime.UtcNow
        };
        
        _notifications.Insert(0, notification);
        return CreatedAtAction(nameof(GetNotifications), new { userId = request.UserId }, notification);
    }
    
    /// <summary>
    /// Get system alerts
    /// </summary>
    [HttpGet("alerts")]
    public ActionResult<List<SystemAlert>> GetAlerts()
    {
        var activeAlerts = _alerts
            .Where(a => a.IsActive && (a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow))
            .OrderByDescending(a => a.Priority)
            .ThenByDescending(a => a.CreatedAt)
            .ToList();
        
        return Ok(activeAlerts);
    }
    
    /// <summary>
    /// Create system alert (admin only)
    /// </summary>
    [HttpPost("alerts")]
    public ActionResult<SystemAlert> CreateAlert([FromBody] CreateAlertRequest request)
    {
        var alert = new SystemAlert
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Message = request.Message,
            Type = request.Type,
            Priority = request.Priority,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = request.ExpiresAt
        };
        
        _alerts.Insert(0, alert);
        return Ok(alert);
    }
    
    /// <summary>
    /// Dismiss alert
    /// </summary>
    [HttpPost("alerts/{id}/dismiss")]
    public ActionResult DismissAlert(Guid id)
    {
        var alert = _alerts.FirstOrDefault(a => a.Id == id);
        if (alert == null) return NotFound();
        
        alert.IsActive = false;
        return Ok();
    }
    
    private static List<Notification> InitializeNotifications()
    {
        return new List<Notification>
        {
            new()
            {
                UserId = "all",
                Title = "Bienvenido a NET10 DeFi",
                Message = "Tu plataforma DeFi soberana está lista. Explora swap, pools y farming.",
                Type = NotificationType.Info,
                Category = "system",
                Priority = NotificationPriority.Normal,
                ActionUrl = "/index.html"
            },
            new()
            {
                UserId = "all",
                Title = "NAGADAN ERP Disponible",
                Message = "El sistema ERP completo está ahora activo. Facturación, contabilidad e inventario.",
                Type = NotificationType.Success,
                Category = "feature",
                Priority = NotificationPriority.High,
                ActionUrl = "/erp.html"
            },
            new()
            {
                UserId = "all",
                Title = "Geocoder Pro Lanzado",
                Message = "Nuevo servicio de geocodificación con soporte para Google API y procesamiento CSV.",
                Type = NotificationType.Info,
                Category = "feature",
                Priority = NotificationPriority.Normal,
                ActionUrl = "/geocoder.html"
            },
            new()
            {
                UserId = "all",
                Title = "Nuevo Pool: IGT-PM/USDT",
                Message = "Pool de liquidez disponible con 85% APR. Añade liquidez ahora.",
                Type = NotificationType.Success,
                Category = "defi",
                Priority = NotificationPriority.High,
                ActionUrl = "/index.html#pools"
            },
            new()
            {
                UserId = "all",
                Title = "Farm Activo: 250% APR",
                Message = "Nuevo farm de alto rendimiento disponible. Stake tus LP tokens.",
                Type = NotificationType.Warning,
                Category = "defi",
                Priority = NotificationPriority.Urgent,
                ActionUrl = "/index.html#farming"
            }
        };
    }
    
    private static List<SystemAlert> InitializeAlerts()
    {
        return new List<SystemAlert>
        {
            new()
            {
                Title = "Mantenimiento Programado",
                Message = "Mantenimiento del sistema el próximo domingo 02:00-04:00 UTC",
                Type = AlertType.Warning,
                Priority = AlertPriority.Medium,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            },
            new()
            {
                Title = "Nuevo: .NET 10 Platform",
                Message = "La plataforma ha sido actualizada a .NET 10 con mejoras de rendimiento.",
                Type = AlertType.Info,
                Priority = AlertPriority.Low,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            }
        };
    }
}

// ═══════════════════════════════════════════════════════════════
// NOTIFICATION MODELS
// ═══════════════════════════════════════════════════════════════

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public string Category { get; set; } = string.Empty;
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public string? ActionUrl { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error
}

public enum NotificationPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public class NotificationResponse
{
    public List<Notification> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int UnreadCount { get; set; }
}

public class CreateNotificationRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public string Category { get; set; } = string.Empty;
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public string? ActionUrl { get; set; }
}

public class SystemAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public AlertType Type { get; set; } = AlertType.Info;
    public AlertPriority Priority { get; set; } = AlertPriority.Medium;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}

public enum AlertType
{
    Info,
    Success,
    Warning,
    Error,
    Critical
}

public enum AlertPriority
{
    Low,
    Medium,
    High,
    Critical
}

public class CreateAlertRequest
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public AlertType Type { get; set; } = AlertType.Info;
    public AlertPriority Priority { get; set; } = AlertPriority.Medium;
    public DateTime? ExpiresAt { get; set; }
}
