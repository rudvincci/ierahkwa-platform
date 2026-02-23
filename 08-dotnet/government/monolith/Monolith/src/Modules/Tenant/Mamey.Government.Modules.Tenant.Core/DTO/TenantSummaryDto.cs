using System;

namespace Mamey.Government.Modules.Tenant.Core.DTO;

public class TenantSummaryDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public bool IsActive { get; set; }
    public string Status => IsActive ? "Active" : "Suspended";
}
