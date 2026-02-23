using Azure.Communication.Email;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Emails.ACS;

public static class FluentEmailACSBuilderExtensions
{
    public static FluentEmailServicesBuilder AddACSEmailSender(this FluentEmailServicesBuilder builder)
    {
        var emailOptions = builder.Services.GetOptions<EmailOptions>("email");
        
        builder.Services.AddSingleton(new EmailClient(emailOptions.ACS.ConnectionString));
        return builder;
    }
}
