using Mamey.CQRS;
using Mamey.Government.Modules.Citizens.Core.Domain.Entities;

namespace Mamey.Government.Modules.Citizens.Core.Domain.Events;

public record CitizenModified(Citizen Citizen) : IDomainEvent;
