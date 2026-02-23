using Mamey.CQRS;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;

namespace Mamey.Government.Modules.TravelIdentities.Core.Domain.Events;

internal record TravelIdentityModified(TravelIdentity TravelIdentity) : IDomainEvent;
