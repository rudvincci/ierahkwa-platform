using System;

namespace Mamey.Government.Modules.Identity.Core.DTO;

public class UserProfileSummaryDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
