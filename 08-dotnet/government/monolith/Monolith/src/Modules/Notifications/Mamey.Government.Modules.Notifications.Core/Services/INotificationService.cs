using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Templates.Types;
using Mamey.Emails.MailKit;

namespace Mamey.Government.Modules.Notifications.Core.Services;

internal interface INotificationService
{
    Task SendAsync(Notification notification);
    Task<bool> SendEmailUsingTemplate<T>(string to, string subject, EmailTemplateType template, T model) where T : IEmailTemplate;
}