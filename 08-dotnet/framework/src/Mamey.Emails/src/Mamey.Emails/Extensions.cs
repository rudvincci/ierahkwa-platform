using Mamey.Emails.ACS;
using Mamey.Emails.MailKit;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Emails;

public static class Extensions
{
    private const string SectionName = "email";

    public static IMameyBuilder AddEmail(this IMameyBuilder builder,
        string sectionName = SectionName)
    {
        builder.Services.AddEmail();
        
        return builder;
    }

    public static IServiceCollection AddEmail(this IServiceCollection services, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var emailOptions = services.GetOptions<EmailOptions>(sectionName);
        services.AddSingleton(emailOptions);
        
        var emailBuilder = services.AddFluentEmail(emailOptions.EmailId);
        _ = emailOptions.Type switch
        {
            "smtp" => emailBuilder.AddSmtpMailkit(emailOptions),
            "acs" => emailBuilder.AddACS(),
            _ => throw new ApplicationException("Invalid email configuration.")
        };
        return services;
    }
}