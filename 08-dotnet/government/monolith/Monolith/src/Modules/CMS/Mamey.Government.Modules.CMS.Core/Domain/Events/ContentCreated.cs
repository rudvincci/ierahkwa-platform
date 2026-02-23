using Mamey.CQRS;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using GovTenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.CMS.Core.Domain.Events;

public record ContentCreated(
    ContentId ContentId,
    GovTenantId TenantId,
    string Title,
    string Slug) : IDomainEvent;
