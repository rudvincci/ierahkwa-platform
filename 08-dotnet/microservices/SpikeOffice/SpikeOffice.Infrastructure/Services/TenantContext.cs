namespace SpikeOffice.Infrastructure.Services;

public class TenantContext : Core.Interfaces.ITenantContext
{
    public Guid? TenantId { get; set; }
    public string? TenantUrlPrefix { get; set; }
    public bool HasTenant => TenantId.HasValue;
}
