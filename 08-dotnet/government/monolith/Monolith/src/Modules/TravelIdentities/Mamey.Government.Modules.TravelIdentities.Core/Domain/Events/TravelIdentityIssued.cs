using System;
using Mamey.CQRS;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.TravelIdentities.Core.Domain.Events;

public record TravelIdentityIssued(
    TravelIdentityId TravelIdentityId,
    CitizenId CitizenId,
    TravelIdentityNumber DocumentNumber,
    Guid ApplicationId = default) : IDomainEvent;
