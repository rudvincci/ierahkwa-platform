using Mamey.CQRS;
using Mamey.ServiceName.Domain.Entities;

namespace Mamey.ServiceName.Domain.Events;

internal record EntityNameCreated(EntityName EntityName) : IDomainEvent;

