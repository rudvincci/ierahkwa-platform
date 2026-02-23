using Mamey.CQRS;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Citizens.Core.Domain.Events;

public record CitizenStatusChanged(
    CitizenId CitizenId,
    CitizenshipStatus OldStatus,
    CitizenshipStatus NewStatus,
    string? Reason = null) : IDomainEvent;
