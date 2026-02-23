namespace Mamey.Government.Modules.Tenant.Core.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for Tenant entities.
/// </summary>
public record TenantId
{
    public Guid Value { get; }

    public TenantId(Guid value)
    {
        Value = value;
    }

    public static TenantId New() => new(Guid.NewGuid());
    
    public static implicit operator Guid(TenantId id) => id.Value;
    public static implicit operator TenantId(Guid id) => new(id);

    public override string ToString() => Value.ToString();
}
