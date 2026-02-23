using Mamey.CQRS;
using Mamey.Government.Modules.Identity.Core.Domain.Entities;

namespace Mamey.Government.Modules.Identity.Core.Domain.Events;

public record UserProfileModified(UserProfile UserProfile) : IDomainEvent;
