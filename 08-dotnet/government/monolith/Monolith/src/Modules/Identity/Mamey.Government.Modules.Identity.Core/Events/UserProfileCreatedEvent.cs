using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Identity.Core.Events;

public record UserProfileCreatedEvent(
    Guid UserId,
    string AuthenticatorIssuer,
    string AuthenticatorSubject,
    string? Email) : IEvent;
