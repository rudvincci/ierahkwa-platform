using Mamey.CQRS;
using Mamey.Government.Modules.CMS.Core.Domain.Entities;

namespace Mamey.Government.Modules.CMS.Core.Domain.Events;

public record ContentModified(Content Content) : IDomainEvent;
