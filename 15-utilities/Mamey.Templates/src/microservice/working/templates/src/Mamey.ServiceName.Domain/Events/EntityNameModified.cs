using Mamey.CQRS;
using Mamey.ServiceName.Domain.Entities;

namespace Mamey.ServiceName.Domain.Events;

internal record EntityNameModified(EntityName EntityName): IDomainEvent;

