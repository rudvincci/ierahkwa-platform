using System.Reflection;
using FluentEmail.Core;
using Mamey.Emails;
using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Application.Templates;
using Mamey.FWID.Notifications.Application.Templates.Models;
using Mamey.FWID.Notifications.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Infrastructure.Services;

/// <summary>
/// Service for sending notifications.
/// </summary>
internal class NotificationService : INotificationService
{
    private const string TemplatePath = "Mamey.FWID.Notifications.Application.Templates.{0}.cshtml";
    private readonly IFluentEmail _email;
    private readonly ILogger<NotificationService> _logger;
    private readonly EmailOptions _emailOptions;
    private readonly IHubContext<Application.Hubs.NotificationHub> _hubContext;

    public NotificationService(
        IFluentEmail email,
        ILogger<NotificationService> logger,
        EmailOptions emailOptions,
        IHubContext<Application.Hubs.NotificationHub> hubContext)
    {
        _email = email;
        _logger = logger;
        _emailOptions = emailOptions;
        _hubContext = hubContext;
    }

    public async Task SendAsync(Notification notification)
    {
        _logger.LogInformation("Sending notification: NotificationId={NotificationId}, Type={Type}, IdentityId={IdentityId}",
            notification.Id.Value, notification.Type, notification.IdentityId.Value);

        try
        {
            // Send via email if Email type is set
            if (notification.Type.HasFlag(NotificationType.Email))
            {
                // Email sending is handled separately via SendEmailUsingTemplate
                _logger.LogInformation("Email notification will be sent separately for NotificationId: {NotificationId}", notification.Id.Value);
            }

            // Send via SMS if Sms type is set
            if (notification.Type.HasFlag(NotificationType.Sms))
            {
                // TODO: Implement SMS sending (Twilio integration)
                _logger.LogInformation("SMS notification not yet implemented for NotificationId: {NotificationId}", notification.Id.Value);
            }

            // Send via Push if Push type is set
            if (notification.Type.HasFlag(NotificationType.Push))
            {
                await SendPushAsync(notification.IdentityId.Value, notification.Title, notification.Message);
            }

            // Send via InApp (SignalR) if InApp type is set
            if (notification.Type.HasFlag(NotificationType.InApp))
            {
                await _hubContext.Clients.Group($"identity-{notification.IdentityId.Value}")
                    .SendAsync("ReceiveNotification", new
                    {
                        Id = notification.Id.Value,
                        Title = notification.Title,
                        Description = notification.Description,
                        Message = notification.Message,
                        Type = notification.Type.ToString(),
                        CreatedAt = notification.CreatedAt
                    });
            }

            _logger.LogInformation("Successfully sent notification: NotificationId={NotificationId}", notification.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification: NotificationId={NotificationId}", notification.Id.Value);
            throw;
        }
    }

    public async Task<bool> SendEmailUsingTemplate<T>(string to, string subject, EmailTemplateType template, T model)
        where T : IEmailTemplate
    {
        try
        {
            var result = await _email
                .SetFrom(_emailOptions.EmailId, _emailOptions.Name)
                .To(to)
                .Subject(subject)
                .UsingTemplateFromEmbedded<T>(
                    string.Format(TemplatePath, template),
                    model,
                    Assembly.GetAssembly(typeof(EmailTemplateType)))
                .SendAsync();

            if (!result.Successful)
            {
                _logger.LogError("Failed to send email to {To}.\n{Errors}",
                    to, string.Join(Environment.NewLine, result.ErrorMessages));
            }

            return result.Successful;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
            return false;
        }
    }

    public Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        // TODO: Implement SMS sending (Twilio integration)
        _logger.LogInformation("SMS sending not yet implemented. PhoneNumber: {PhoneNumber}, Message: {Message}",
            phoneNumber, message);
        return Task.FromResult(false);
    }

    public Task<bool> SendPushAsync(Guid identityId, string title, string message)
    {
        // TODO: Implement Push notification (Firebase/APNs integration)
        _logger.LogInformation("Push notification not yet implemented. IdentityId: {IdentityId}, Title: {Title}",
            identityId, title);
        return Task.FromResult(false);
    }
}







