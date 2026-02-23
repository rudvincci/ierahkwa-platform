using Mamey.CQRS;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Passports.Core.Domain.Events;

public record PassportRevoked(
    PassportId PassportId,
    string Reason) : IDomainEvent;
