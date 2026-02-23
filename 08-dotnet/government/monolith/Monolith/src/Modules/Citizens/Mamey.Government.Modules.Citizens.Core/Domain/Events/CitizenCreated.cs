using Mamey.CQRS;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Modules.Citizens.Core.Domain.Events;

public record CitizenCreated(
    CitizenId CitizenId,
    TenantId TenantId,
    Name CitizenName,
    CitizenshipStatus Status,
    Guid ApplicationId = default) : IDomainEvent;
