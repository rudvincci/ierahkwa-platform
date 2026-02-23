using Mamey.CQRS.Commands;
using Mamey.MicroMonolith.Abstractions.Contracts;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.Government.Modules.Notifications.Core.Commands;

internal record SendCitizenshipApplicationLink(
    string Email,
    string ApplicationUrl,
    string? FirstName = null,
    string? LastName = null) : ICommand;

[Message("citizenship-applications")]
internal class SendCitizenshipApplicationLinkContract : Contract<SendCitizenshipApplicationLink>
{
    public SendCitizenshipApplicationLinkContract()
    {
        RequireAll();
    }
}
