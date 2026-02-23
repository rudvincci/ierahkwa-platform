namespace SpikeOffice.Core.Interfaces;

public interface ITenantContext
{
    Guid? TenantId { get; }
    string? TenantUrlPrefix { get; }
    bool HasTenant { get; }
}
