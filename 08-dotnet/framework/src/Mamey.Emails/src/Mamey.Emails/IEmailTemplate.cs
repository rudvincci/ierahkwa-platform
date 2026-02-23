using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mamey.Types;

namespace Mamey.Emails.MailKit;

public interface IEmailTemplate
{
    // Marker
}

public interface IBusinessEmailTemplate : IEmailTemplate
{
    public string Title { get; }
    public string CompanyName { get; set; }
    public Address CompanyAddress { get; set; }
    public Name RecipientName { get; set; }
}

public class BusinessEmailTemplate(string title, string companyName, Address companyAddress, Name recipientName, string supportUrl) : IBusinessEmailTemplate
{
    public string Title { get; } = title;
    public string CompanyName { get; set; } = companyName;
    public Address CompanyAddress { get; set; } = companyAddress;
    public Name RecipientName { get; set; } = recipientName;
    public string SupportUrl { get; set; } = supportUrl;
}
public interface IMameyEmailService
{
    Task SendAsyncT<T>(T notification);
    Task<bool> SendEmailUsingTemplate<T>(string to, string subject, string template, T model, bool isHtml = true) where T : IEmailTemplate;
    Task<bool> SendEmailUsingTemplateFromEmbedded<T>(string templateEmbeddedResourcePath, string to, string subject, Assembly assembly, T model) where T : IEmailTemplate;
}
public class MameyEmailService
{
    private readonly ILogger<MameyEmailService> _logger;
    private readonly IFluentEmail _email;
    private readonly EmailOptions _options;

    public MameyEmailService(IFluentEmail email, ILogger<MameyEmailService> logger, EmailOptions options)
    {
        _email = email;
        _logger = logger;
        _options = options;
    }

    public Task SendAsync<T>(T notification)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SendEmailUsingTemplateFromEmbedded<T>(string templateEmbeddedResourcePath,
        string to, string subject, Assembly assembly, T model)
        where T : IEmailTemplate
    {
        var result = await _email
        .SetFrom(_options.EmailId, _options.Name)
        .To(to)
        .Subject(subject)
        .UsingTemplateFromEmbedded<T>(templateEmbeddedResourcePath, model, assembly)
        .SendAsync();

        if (!result.Successful)
        {
            _logger.LogError("Failed to send an email.\n{Errors}",
                string.Join(Environment.NewLine, result.ErrorMessages));
        }

        return result.Successful;
    }
    public async Task<bool> SendEmailUsingTemplate<T>(string to, string subject,
        string template, T model, bool isHtml = true)
        where T : IEmailTemplate
    {
        var result = await _email
        .SetFrom(_options.EmailId, _options.Name)
        .To(to)
        .Subject(subject)
        .UsingTemplate<T>(template, model, isHtml)
        .SendAsync();

        if (!result.Successful)
        {
            _logger.LogError("Failed to send an email.\n{Errors}",
                string.Join(Environment.NewLine, result.ErrorMessages));
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