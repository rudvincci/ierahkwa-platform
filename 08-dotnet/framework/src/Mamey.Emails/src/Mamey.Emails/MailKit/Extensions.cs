using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Emails.MailKit;

public static class Extensions
{
    private const string SectionName = "email:mailkit";

    public static FluentEmailServicesBuilder AddSmtpMailkit(this FluentEmailServicesBuilder emailBuilder,
        EmailOptions emailOptions, string sectionName = SectionName)
    {
        emailBuilder
        // emailBuilder
            .AddRazorRenderer()
            //.AddSmtpSender("localhost", 25);
            .AddSmtpSender(new SmtpClient(emailOptions.Mailkit.Smtp, emailOptions.Mailkit.Port)
            {
                Credentials = new NetworkCredential(emailOptions.EmailId, emailOptions.Mailkit.Password),
                EnableSsl = emailOptions.Mailkit.UseSsl
            });
        return emailBuilder;
    }
}