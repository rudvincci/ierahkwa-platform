namespace Mamey.Portal.Tenant.Domain.ValueObjects;

public readonly record struct TenantId(string Value)
{
    public override string ToString() => Value;
}
