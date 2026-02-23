using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Events;


public record ApplicationApprovedEvent(
    Guid ApplicationId,
    string ApplicationNumber,
    string ApprovedBy,
    DateTime ApprovedAt) : IEvent;
