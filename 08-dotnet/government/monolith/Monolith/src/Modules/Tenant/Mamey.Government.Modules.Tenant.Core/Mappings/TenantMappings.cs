using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Government.Modules.Tenant.Core.DTO;

namespace Mamey.Government.Modules.Tenant.Core.Mappings;

internal static class TenantMappings
{
    public static TenantDto AsDto(this TenantEntity tenant)
        => new()
        {
            Id = tenant.Id.Value,
            DisplayName = tenant.DisplayName,
            Domain = tenant.Domain,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt
        };

    public static TenantSummaryDto AsSummaryDto(this TenantEntity tenant)
        => new()
        {
            Id = tenant.Id.Value,
            DisplayName = tenant.DisplayName,
            Domain = tenant.Domain,
            IsActive = tenant.IsActive
        };
}
