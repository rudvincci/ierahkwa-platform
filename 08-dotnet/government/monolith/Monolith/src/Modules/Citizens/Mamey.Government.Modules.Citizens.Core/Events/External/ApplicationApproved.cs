using Mamey.MicroMonolith.Abstractions.Contracts;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.Government.Modules.Citizens.Core.Events.External;

internal record ApplicationApproved(Guid ApplicationId, string ApplicationNumber, string ApprovedBy, DateTime ApprovedAt);

[Message("citizenship-applications")]
internal class ApplicationApprovedContract : Contract<ApplicationApproved>
{
    public ApplicationApprovedContract()
    {
        RequireAll();
    }
}