using Mamey.CQRS;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Documents.Core.Domain.Events;

public record DocumentDeleted(DocumentId DocumentId) : IDomainEvent;
