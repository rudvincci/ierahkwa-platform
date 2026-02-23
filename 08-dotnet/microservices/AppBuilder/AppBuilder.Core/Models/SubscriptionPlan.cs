namespace AppBuilder.Core.Models;

/// <summary>Subscription plan - Free, Pro, Enterprise. Appy: Subscription Plans, Build Credits, Premium Features.</summary>
public class SubscriptionPlan
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;       // Free, Pro, Enterprise
    public PlanTier Tier { get; set; }
    public decimal PriceMonthly { get; set; }
    public decimal PriceYearly { get; set; }
    public int BuildCreditsPerMonth { get; set; }
    public bool UnlimitedBuilds { get; set; }
    public string[] Features { get; set; } = Array.Empty<string>();
    public bool IsActive { get; set; } = true;
}

/// <summary>User subscription - current plan, renewal, payment method.</summary>
public class Subscription
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime? CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum SubscriptionStatus
{
    Active,
    PastDue,
    Cancelled,
    Trial
}
