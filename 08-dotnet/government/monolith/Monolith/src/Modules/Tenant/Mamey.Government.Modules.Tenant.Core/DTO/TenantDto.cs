using System;

namespace Mamey.Government.Modules.Tenant.Core.DTO;

public class TenantDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status => IsActive ? "Active" : "Suspended";
}
