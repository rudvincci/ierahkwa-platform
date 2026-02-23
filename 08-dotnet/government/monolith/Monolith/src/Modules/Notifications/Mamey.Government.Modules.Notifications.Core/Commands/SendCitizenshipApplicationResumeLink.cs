using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Notifications.Core.Commands;

internal record SendCitizenshipApplicationResumeLink(
    string Email,
    string ApplicationUrl,
    string? FirstName = null,
    string? LastName = null) : ICommand;
