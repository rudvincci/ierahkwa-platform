using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Events;

public record ApplicationRejectedEvent(
    Guid ApplicationId,
    string ApplicationNumber,
    string Reason,
    string RejectedBy,
    DateTime RejectedAt) : IEvent;
