using Mamey.Government.Modules.Tenant.Core.Domain.Events;
using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Domain.Entities;

/// <summary>
/// Tenant entity for multi-tenancy support.
/// Each tenant represents a separate organization/government entity.
/// </summary>
public class TenantEntity : AggregateRoot<TenantId>
{
    private TenantEntity() { }

    public TenantEntity(
        TenantId id,
        string displayName,
        string? domain = null,
        bool isActive = true,
        int version = 0)
        : base(id, version)
    {
        DisplayName = displayName;
        Domain = domain;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new TenantCreated(Id, DisplayName));
    }

    public string DisplayName { get; private set; } = string.Empty;
    public string? Domain { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void UpdateDisplayName(string displayName)
    {
        DisplayName = displayName;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new TenantModified(this));
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new TenantModified(this));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new TenantModified(this));
    }
}
