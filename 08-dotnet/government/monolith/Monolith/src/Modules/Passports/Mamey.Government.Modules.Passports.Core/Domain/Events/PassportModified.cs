using Mamey.CQRS;
using Mamey.Government.Modules.Passports.Core.Domain.Entities;

namespace Mamey.Government.Modules.Passports.Core.Domain.Events;

internal record PassportModified(Passport Passport) : IDomainEvent;
