using Mamey.Types;

namespace Mamey.FWID.Notifications.Application.Templates.Models;

/// <summary>
/// Email template model for welcome emails.
/// </summary>
internal record WelcomeDto(Name Name, string VerificationToken) : IEmailTemplate;







