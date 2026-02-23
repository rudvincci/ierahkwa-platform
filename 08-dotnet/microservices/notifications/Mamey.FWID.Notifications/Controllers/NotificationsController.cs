using Microsoft.AspNetCore.Mvc;
using Mamey.FWID.Notifications.Models;
using Mamey.FWID.Notifications.Services;

namespace Mamey.FWID.Notifications.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    
    public NotificationsController(INotificationService service) => _service = service;
    
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
    {
        var notification = await _service.SendAsync(request);
        return Ok(new { success = true, notification });
    }
    
    [HttpPost("send-template")]
    public async Task<IActionResult> SendFromTemplate([FromBody] SendTemplateRequest request)
    {
        var notification = await _service.SendFromTemplateAsync(request.TemplateName, request.Variables, request.RecipientIds);
        return Ok(new { success = true, notification });
    }
    
    [HttpPost("email")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        var result = await _service.SendEmailAsync(request.To, request.Subject, request.Body, request.IsHtml);
        return Ok(new { success = result });
    }
    
    [HttpPost("sms")]
    public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
    {
        var result = await _service.SendSmsAsync(request.To, request.Message);
        return Ok(new { success = result });
    }
    
    [HttpPost("push")]
    public async Task<IActionResult> SendPush([FromBody] PushRequest request)
    {
        var result = await _service.SendPushAsync(request.Token, request.Title, request.Body, request.Data);
        return Ok(new { success = result });
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var notification = await _service.GetAsync(id);
        return notification != null ? Ok(new { notification }) : NotFound();
    }
    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetForUser(string userId, [FromQuery] int limit = 50)
    {
        var notifications = await _service.GetForUserAsync(userId, limit);
        return Ok(new { notifications, count = notifications.Count });
    }
    
    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, [FromQuery] string userId)
    {
        var result = await _service.MarkAsReadAsync(id, userId);
        return Ok(new { success = result });
    }
    
    [HttpPost("user/{userId}/read-all")]
    public async Task<IActionResult> MarkAllAsRead(string userId)
    {
        var result = await _service.MarkAllAsReadAsync(userId);
        return Ok(new { success = result });
    }
    
    [HttpGet("templates/{name}")]
    public async Task<IActionResult> GetTemplate(string name)
    {
        var template = await _service.GetTemplateAsync(name);
        return template != null ? Ok(new { template }) : NotFound();
    }
    
    [HttpPost("templates")]
    public async Task<IActionResult> CreateTemplate([FromBody] NotificationTemplate template)
    {
        var result = await _service.CreateTemplateAsync(template);
        return Ok(new { success = true, template = result });
    }
    
    [HttpGet("preferences/{userId}")]
    public async Task<IActionResult> GetPreferences(string userId)
    {
        var prefs = await _service.GetPreferencesAsync(userId);
        return Ok(new { preferences = prefs });
    }
    
    [HttpPut("preferences/{userId}")]
    public async Task<IActionResult> UpdatePreferences(string userId, [FromBody] UserPreferences prefs)
    {
        var result = await _service.UpdatePreferencesAsync(userId, prefs);
        return Ok(new { success = true, preferences = result });
    }
}

public record SendTemplateRequest(string TemplateName, Dictionary<string, string> Variables, List<string> RecipientIds);
public record EmailRequest(string To, string Subject, string Body, bool IsHtml = false);
public record SmsRequest(string To, string Message);
public record PushRequest(string Token, string Title, string Body, Dictionary<string, string>? Data = null);
