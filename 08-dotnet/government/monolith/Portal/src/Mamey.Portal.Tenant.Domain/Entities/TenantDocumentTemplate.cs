using Mamey.Portal.Tenant.Domain.ValueObjects;

namespace Mamey.Portal.Tenant.Domain.Entities;

public sealed class TenantDocumentTemplate
{
    public TenantId TenantId { get; private set; }
    public string Kind { get; private set; } = string.Empty;
    public string TemplateHtml { get; private set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; private set; }

    private TenantDocumentTemplate() { }

    public TenantDocumentTemplate(TenantId tenantId, string kind, string templateHtml, DateTimeOffset updatedAt)
    {
        TenantId = tenantId;
        Kind = kind;
        TemplateHtml = templateHtml;
        UpdatedAt = updatedAt;
    }
}
