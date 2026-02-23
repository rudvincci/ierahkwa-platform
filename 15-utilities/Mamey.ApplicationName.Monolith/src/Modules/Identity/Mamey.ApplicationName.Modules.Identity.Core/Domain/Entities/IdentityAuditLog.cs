using Mamey.Auth.Identity.Entities;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Core.Domain.Entities;

public class IdentityAuditLog : AuditLog
{
    public IdentityAuditLog(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; private set; }

    public string EntityName { get; set; } = default!;
    public string EntityId { get; set; } = default!;
    public string Action { get; set; } = default!;
    public Guid ChangedBy { get; set; } = default!;
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public string? OriginalValues { get; set; }
    public string? NewValues { get; set; }
}

