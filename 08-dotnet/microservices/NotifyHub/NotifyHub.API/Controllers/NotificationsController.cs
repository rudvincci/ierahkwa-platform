using Microsoft.AspNetCore.Mvc;
using NotifyHub.Core.Interfaces;
using NotifyHub.Core.Models;
namespace NotifyHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    public NotificationsController(INotificationService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Notification>> Send([FromBody] Notification notification) => await _service.SendNotificationAsync(notification);
    [HttpPost("schedule")] public async Task<ActionResult<Notification>> Schedule([FromBody] Notification notification) => await _service.ScheduleNotificationAsync(notification);
    [HttpGet("{id}")] public async Task<ActionResult<Notification>> GetById(Guid id) { var n = await _service.GetNotificationByIdAsync(id); return n == null ? NotFound() : n; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Notification>>> GetAll([FromQuery] NotificationStatus? status, [FromQuery] NotificationType? type) => Ok(await _service.GetNotificationsAsync(status, type));
    [HttpPost("{id}/cancel")] public async Task<ActionResult<Notification>> Cancel(Guid id) => await _service.CancelNotificationAsync(id);
    [HttpPost("template/{templateName}")] public async Task<ActionResult<Notification>> SendFromTemplate(string templateName, [FromBody] SendTemplateRequest request) => await _service.SendFromTemplateAsync(templateName, request.Variables, request.RecipientUserIds);
    [HttpGet("statistics")] public async Task<ActionResult<NotificationStatistics>> GetStatistics() => await _service.GetStatisticsAsync();
}

public class SendTemplateRequest { public Dictionary<string, string> Variables { get; set; } = new(); public List<Guid> RecipientUserIds { get; set; } = new(); }

[ApiController]
[Route("api/users/{userId}/notifications")]
public class UserNotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    public UserNotificationsController(INotificationService service) => _service = service;

    [HttpGet] public async Task<ActionResult<IEnumerable<NotificationDelivery>>> GetAll(Guid userId, [FromQuery] bool? unreadOnly, [FromQuery] int page = 1, [FromQuery] int pageSize = 20) => Ok(await _service.GetUserNotificationsAsync(userId, unreadOnly, page, pageSize));
    [HttpGet("unread-count")] public async Task<ActionResult<int>> GetUnreadCount(Guid userId) => await _service.GetUnreadCountAsync(userId);
    [HttpPost("{notificationId}/read")] public async Task<ActionResult> MarkAsRead(Guid userId, Guid notificationId) { await _service.MarkAsReadAsync(userId, notificationId); return Ok(); }
    [HttpPost("read-all")] public async Task<ActionResult> MarkAllAsRead(Guid userId) { await _service.MarkAllAsReadAsync(userId); return Ok(); }
    [HttpDelete("{notificationId}")] public async Task<ActionResult> Delete(Guid userId, Guid notificationId) { await _service.DeleteNotificationAsync(userId, notificationId); return NoContent(); }
    [HttpGet("preferences")] public async Task<ActionResult<UserPreference>> GetPreferences(Guid userId) => await _service.GetUserPreferencesAsync(userId);
    [HttpPut("preferences")] public async Task<ActionResult<UserPreference>> UpdatePreferences(Guid userId, [FromBody] UserPreference preferences) { preferences.UserId = userId; return await _service.UpdateUserPreferencesAsync(preferences); }
    [HttpPost("push-token")] public async Task<ActionResult> RegisterPushToken(Guid userId, [FromBody] string token) { await _service.RegisterPushTokenAsync(userId, token); return Ok(); }
    [HttpGet("subscriptions")] public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions(Guid userId) => Ok(await _service.GetUserSubscriptionsAsync(userId));
    [HttpPost("subscriptions")] public async Task<ActionResult<Subscription>> Subscribe(Guid userId, [FromBody] SubscribeRequest request) => await _service.SubscribeToTopicAsync(userId, request.Topic, request.Channels);
    [HttpDelete("subscriptions/{topic}")] public async Task<ActionResult> Unsubscribe(Guid userId, string topic) { await _service.UnsubscribeFromTopicAsync(userId, topic); return NoContent(); }
}

public class SubscribeRequest { public string Topic { get; set; } = string.Empty; public List<DeliveryChannel> Channels { get; set; } = new(); }

[ApiController]
[Route("api/notification-templates")]
public class TemplatesController : ControllerBase
{
    private readonly INotificationService _service;
    public TemplatesController(INotificationService service) => _service = service;
    [HttpPost] public async Task<ActionResult<NotificationTemplate>> Create([FromBody] NotificationTemplate template) => await _service.CreateTemplateAsync(template);
    [HttpGet] public async Task<ActionResult<IEnumerable<NotificationTemplate>>> GetAll([FromQuery] NotificationType? type) => Ok(await _service.GetTemplatesAsync(type));
    [HttpGet("{id}")] public async Task<ActionResult<NotificationTemplate>> GetById(Guid id) { var t = await _service.GetTemplateByIdAsync(id); return t == null ? NotFound() : t; }
    [HttpPut("{id}")] public async Task<ActionResult<NotificationTemplate>> Update(Guid id, [FromBody] NotificationTemplate template) => await _service.UpdateTemplateAsync(template);
}
