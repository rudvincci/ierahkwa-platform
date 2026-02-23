using Mamey.CQRS;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Modules.Documents.Core.Domain.Events;

public record DocumentCreated(
    DocumentId DocumentId,
    TenantId TenantId,
    string FileName) : IDomainEvent;
