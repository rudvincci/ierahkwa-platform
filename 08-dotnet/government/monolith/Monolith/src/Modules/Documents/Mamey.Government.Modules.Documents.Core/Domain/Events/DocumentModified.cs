using Mamey.CQRS;
using Mamey.Government.Modules.Documents.Core.Domain.Entities;

namespace Mamey.Government.Modules.Documents.Core.Domain.Events;

internal record DocumentModified(Document Document) : IDomainEvent;
