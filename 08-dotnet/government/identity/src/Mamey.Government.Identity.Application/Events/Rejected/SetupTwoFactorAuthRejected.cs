using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.Government.Identity.Application.Events.Rejected;

[Contract]
internal record SetupTwoFactorAuthRejected(Guid TwoFactorAuthId, string Reason, string Code) : IRejectedEvent;
