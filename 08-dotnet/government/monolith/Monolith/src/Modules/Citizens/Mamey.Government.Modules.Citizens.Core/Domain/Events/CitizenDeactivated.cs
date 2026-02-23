using Mamey.CQRS;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Citizens.Core.Domain.Events;

public record CitizenDeactivated(
    CitizenId CitizenId,
    string Reason) : IDomainEvent;
