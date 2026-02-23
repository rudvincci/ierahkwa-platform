using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Identity.Core.Events;

public record UserProfileUpdatedEvent(Guid UserId, string? Email, string? DisplayName) : IEvent;
