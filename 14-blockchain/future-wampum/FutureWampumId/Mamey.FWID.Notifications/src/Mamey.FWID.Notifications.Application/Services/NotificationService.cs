using Mamey.FWID.Notifications.Application.Templates;
using Mamey.FWID.Notifications.Application.Templates.Models;
using Mamey.FWID.Notifications.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Services;

/// <summary>
/// Service for sending notifications via multiple channels.
/// </summary>
internal sealed class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly INotificationStorageService _storageService;
    
    public NotificationService(
        ILogger<NotificationService> logger,
        INotificationStorageService storageService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    }
    
    /// <inheritdoc />
    public async Task SendAsync(Notification notification)
    {
        _logger.LogInformation("Sending notification {NotificationId} to identity {IdentityId}",
            notification.Id, notification.IdentityId);
        
        try
        {
            // Store the notification
            await _storageService.StoreAsync(notification);
            
            // Send via appropriate channel
            var success = notification.Channel switch
            {
                NotificationChannel.Email => await SendEmailAsync(notification),
                NotificationChannel.SMS => await SendSmsAsync(notification.Recipient ?? "", notification.Message),
                NotificationChannel.Push => await SendPushAsync(notification.IdentityId.Value, notification.Title, notification.Message),
                NotificationChannel.InApp => true, // Already stored
                _ => throw new NotSupportedException($"Channel {notification.Channel} not supported")
            };
            
            if (success)
            {
                notification.MarkAsSent();
                await _storageService.UpdateAsync(notification);
                _logger.LogInformation("Notification {NotificationId} sent successfully via {Channel}",
                    notification.Id, notification.Channel);
            }
            else
            {
                notification.MarkAsFailed("Delivery failed");
                await _storageService.UpdateAsync(notification);
                _logger.LogWarning("Notification {NotificationId} delivery failed", notification.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification {NotificationId}", notification.Id);
            notification.MarkAsFailed(ex.Message);
            await _storageService.UpdateAsync(notification);
        }
    }
    
    /// <inheritdoc />
    public async Task<bool> SendEmailUsingTemplate<T>(string to, string subject, EmailTemplateType template, T model) 
        where T : IEmailTemplate
    {
        _logger.LogInformation("Sending email to {To} using template {Template}", to, template);
        
        try
        {
            // In production, use Mamey.Email library with Razor template rendering
            var body = await RenderTemplateAsync(template, model);
            
            // Simulate email sending
            await Task.Delay(100);
            
            _logger.LogInformation("Email sent to {To} with subject: {Subject}", to, subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
            return false;
        }
    }
    
    /// <inheritdoc />
    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            _logger.LogWarning("Cannot send SMS: phone number is empty");
            return false;
        }
        
        _logger.LogInformation("Sending SMS to {PhoneNumber}", MaskPhoneNumber(phoneNumber));
        
        try
        {
            // In production, use SMS gateway (Twilio, etc.)
            await Task.Delay(100);
            
            _logger.LogInformation("SMS sent to {PhoneNumber}", MaskPhoneNumber(phoneNumber));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {PhoneNumber}", MaskPhoneNumber(phoneNumber));
            return false;
        }
    }
    
    /// <inheritdoc />
    public async Task<bool> SendPushAsync(Guid identityId, string title, string message)
    {
        _logger.LogInformation("Sending push notification to identity {IdentityId}", identityId);
        
        try
        {
            // In production, use Firebase Cloud Messaging or similar
            await Task.Delay(50);
            
            _logger.LogInformation("Push notification sent to {IdentityId}", identityId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification to {IdentityId}", identityId);
            return false;
        }
    }
    
    #region Private Methods
    
    private Task<bool> SendEmailAsync(Notification notification)
    {
        if (string.IsNullOrEmpty(notification.Recipient))
        {
            _logger.LogWarning("Cannot send email: recipient is empty");
            return Task.FromResult(false);
        }
        
        // In production, send actual email
        _logger.LogInformation("Email sent to {Recipient}: {Title}", notification.Recipient, notification.Title);
        return Task.FromResult(true);
    }
    
    private Task<string> RenderTemplateAsync<T>(EmailTemplateType template, T model) where T : IEmailTemplate
    {
        // Simplified - in production use Razor template engine
        var body = template switch
        {
            EmailTemplateType.Welcome => $"<h1>Welcome {model.GetType().GetProperty("Name")?.GetValue(model)}</h1>",
            EmailTemplateType.CredentialIssued => $"<h1>Your credential has been issued</h1>",
            EmailTemplateType.DIDCreated => $"<h1>Your DID has been created</h1>",
            EmailTemplateType.CredentialExpiring => $"<h1>Your credential is expiring soon</h1>",
            EmailTemplateType.ApprovalRequired => $"<h1>Your approval is required</h1>",
            EmailTemplateType.SecurityAlert => $"<h1>Security Alert</h1>",
            _ => $"<h1>Notification</h1>"
        };
        
        return Task.FromResult(body);
    }
    
    private static string MaskPhoneNumber(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 4)
            return "****";
        return phone[..3] + new string('*', phone.Length - 6) + phone[^3..];
    }
    
    #endregion
}

/// <summary>
/// Notification channel types.
/// </summary>
public enum NotificationChannel
{
    Email = 1,
    SMS = 2,
    Push = 3,
    InApp = 4
}
