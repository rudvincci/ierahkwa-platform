using Mamey.Emails.MailKit;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Templates.Models;

public class WelcomeDto(string companyName, Address companyAddress, Name recipientName, string supportUrl, string confirmUrl) 
    : BusinessEmailTemplate("Welcome", companyName, companyAddress, recipientName, supportUrl )
{
    public string ConfirmUrl { get; set; } = confirmUrl;
}