using Mamey.Emails.MailKit;
using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Templates.Models;

public class CitizenshipApplicationResume(
    string companyName,
    Address companyAddress,
    Name recipientName,
    string supportUrl,
    string applicationUrl)
    : BusinessEmailTemplate("Resume Citizenship Application", companyName, companyAddress, recipientName, supportUrl)
{
    public string ApplicationUrl { get; set; } = applicationUrl;
}
