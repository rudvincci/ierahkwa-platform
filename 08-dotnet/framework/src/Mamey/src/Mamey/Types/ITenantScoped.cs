namespace Mamey.Types;


/// <summary>
/// Interface for tenant-scoped entities.
/// </summary>
public interface ITenantScoped
{
    TenantId TenantId { get; }
}