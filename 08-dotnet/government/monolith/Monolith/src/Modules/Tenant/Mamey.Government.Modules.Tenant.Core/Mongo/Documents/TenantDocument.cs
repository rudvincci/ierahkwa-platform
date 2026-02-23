using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Mongo.Documents;

internal class TenantDocument : MicroMonolith.Infrastructure.Mongo.IIdentifiable<Guid>
{
    public TenantDocument()
    {
    }

    public TenantDocument(TenantEntity tenant)
    {
        Id = tenant.Id.Value;
        DisplayName = tenant.DisplayName;
        Domain = tenant.Domain;
        IsActive = tenant.IsActive;
        CreatedAt = tenant.CreatedAt;
        UpdatedAt = tenant.UpdatedAt;
    }

    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public TenantEntity AsEntity()
    {
        var tenantId = new TenantId(Id);
        var tenant = new TenantEntity(
            tenantId,
            DisplayName,
            Domain,
            IsActive);
        
        typeof(TenantEntity).GetProperty("CreatedAt")?.SetValue(tenant, CreatedAt);
        typeof(TenantEntity).GetProperty("UpdatedAt")?.SetValue(tenant, UpdatedAt);
        
        return tenant;
    }
}
