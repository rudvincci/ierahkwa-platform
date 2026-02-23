using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Notifications.Application.Events.Integration.ZKPs;

/// <summary>
/// Integration event raised when a ZKP proof is generated in the ZKPs service.
/// </summary>
[Message("zkps")]
public record ZKPProofGeneratedIntegrationEvent(Guid ProofId, Guid IdentityId, string AttributeType, DateTime GeneratedAt) : IEvent;







