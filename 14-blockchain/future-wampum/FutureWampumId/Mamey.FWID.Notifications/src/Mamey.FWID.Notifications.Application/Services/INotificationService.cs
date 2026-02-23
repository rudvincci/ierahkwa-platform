using Mamey.FWID.Notifications.Application.Templates;
using Mamey.FWID.Notifications.Application.Templates.Models;
using Mamey.FWID.Notifications.Domain.Entities;

namespace Mamey.FWID.Notifications.Application.Services;

/// <summary>
/// Service for sending notifications.
/// </summary>
internal interface INotificationService
{
    /// <summary>
    /// Sends a notification.
    /// </summary>
    Task SendAsync(Notification notification);

    /// <summary>
    /// Sends an email using a template.
    /// </summary>
    Task<bool> SendEmailUsingTemplate<T>(string to, string subject, EmailTemplateType template, T model) where T : IEmailTemplate;

    /// <summary>
    /// Sends an SMS message.
    /// </summary>
    Task<bool> SendSmsAsync(string phoneNumber, string message);

    /// <summary>
    /// Sends a push notification.
    /// </summary>
    Task<bool> SendPushAsync(Guid identityId, string title, string message);
}







