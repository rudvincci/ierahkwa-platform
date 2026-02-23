using System;
using Mamey.CQRS;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Passports.Core.Domain.Events;

public record PassportIssued(
    PassportId PassportId,
    CitizenId CitizenId,
    PassportNumber PassportNumber,
    Guid ApplicationId = default) : IDomainEvent;
