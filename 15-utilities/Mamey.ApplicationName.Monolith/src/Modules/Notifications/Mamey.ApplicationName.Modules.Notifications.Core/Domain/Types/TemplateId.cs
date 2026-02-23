using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Domain.Types;

internal class TemplateId : AggregateId<Guid>
{
    public TemplateId(OrganizationId organizationId)
        : base(Guid.NewGuid())
        => OrganizationId = organizationId;
    public TemplateId(Guid templateId, OrganizationId organizationId)
        : base(templateId)
        => OrganizationId = organizationId;

    public OrganizationId OrganizationId { get; }
}