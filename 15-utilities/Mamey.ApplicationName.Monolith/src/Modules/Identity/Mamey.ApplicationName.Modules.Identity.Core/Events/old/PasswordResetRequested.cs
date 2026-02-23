using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

public record PasswordResetRequested(Guid UserId, Guid OrganizationId, string Token) : IEvent;