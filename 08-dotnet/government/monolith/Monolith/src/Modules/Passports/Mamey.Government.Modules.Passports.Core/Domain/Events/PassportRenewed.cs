using Mamey.CQRS;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Passports.Core.Domain.Events;

public record PassportRenewed(
    PassportId PassportId,
    DateTime NewExpiryDate) : IDomainEvent;
