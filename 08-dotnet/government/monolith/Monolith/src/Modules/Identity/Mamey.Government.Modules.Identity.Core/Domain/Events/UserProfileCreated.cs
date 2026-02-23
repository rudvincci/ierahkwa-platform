using Mamey.CQRS;
using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Domain.Events;

public record UserProfileCreated(
    UserId UserId,
    string AuthenticatorIssuer,
    string AuthenticatorSubject) : IDomainEvent;
