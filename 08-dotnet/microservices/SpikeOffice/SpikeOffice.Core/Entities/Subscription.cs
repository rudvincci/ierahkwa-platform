using SpikeOffice.Core.Enums;

namespace SpikeOffice.Core.Entities;

/// <summary>
/// Subscription-based billing per tenant.
/// </summary>
public class Subscription : BaseEntity
{
    public new Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public SubscriptionPlan Plan { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsTrial { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal? MonthlyAmount { get; set; }
    public int? MaxEmployees { get; set; }
    public string? PaymentGateway { get; set; }
    public string? ExternalSubscriptionId { get; set; }
}
