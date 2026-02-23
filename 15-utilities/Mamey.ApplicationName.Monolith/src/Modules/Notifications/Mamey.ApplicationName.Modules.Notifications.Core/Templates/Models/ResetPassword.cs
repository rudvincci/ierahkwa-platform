using Mamey.Emails.MailKit;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Templates.Models;

public class ResetPassword(string companyName, Address companyAddress, Name recipientName, string supportUrl, string resetUrl) 
    : BusinessEmailTemplate("Reset Password", companyName, companyAddress, recipientName, supportUrl)
{
    public string ResetUrl { get; set; } = resetUrl;
}
