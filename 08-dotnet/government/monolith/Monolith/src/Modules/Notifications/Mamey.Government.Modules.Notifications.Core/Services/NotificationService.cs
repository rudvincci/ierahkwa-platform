using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentEmail.Core;
using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Templates.Types;
using Mamey.Emails;
using Mamey.Emails.MailKit;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Notifications.Core.Services;

internal class NotificationService : INotificationService
{
    private const string TemplatePath = "Mamey.Government.Modules.Notifications.Core.Templates.{0}.cshtml";
    private readonly IFluentEmail _email;
    private readonly ILogger<NotificationService> _logger;
    private readonly EmailOptions _options;

    public NotificationService(IFluentEmail email, ILogger<NotificationService> logger, EmailOptions options)
    {
        _email = email;
        _logger = logger;
        _options = options;
    }

    public Task SendAsync(Notification notification)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SendEmailUsingTemplate<T>(string to, string subject, EmailTemplateType template, T model)
        where T : IEmailTemplate
    {
        var result = await _email
            .SetFrom(_options.EmailId, _options.Name)
            .To(to)
            .Subject(subject)
            .UsingTemplateFromEmbedded<T>(string.Format(TemplatePath, template), model, Assembly.GetAssembly(typeof(EmailTemplateType)))
            .SendAsync();

        if (!result.Successful)
        {
            _logger.LogError("Failed to send an email.\n{Errors}", string.Join(Environment.NewLine, result.ErrorMessages));
        }

        return result.Successful;
    }

    private static ExpandoObject ToExpando(object model)
    {
        if (model is ExpandoObject exp)
        {
            return exp;
        }

        IDictionary<string, object> expando = new ExpandoObject();

        foreach (var propertyDescriptor in model.GetType().GetTypeInfo().GetProperties())
        {
            var obj = propertyDescriptor.GetValue(model);

            if (obj != null && IsAnonymousType(obj.GetType()))
            {
                obj = ToExpando(obj);
            }

            expando.Add(propertyDescriptor.Name, obj);
        }

        return (ExpandoObject)expando;
    }

    private static bool IsAnonymousType(Type type)
    {
        bool hasCompilerGeneratedAttribute = type.GetTypeInfo()
            .GetCustomAttributes(typeof(CompilerGeneratedAttribute), false)
            .Any();

        bool nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
        bool isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

        return isAnonymousType;
    }
}