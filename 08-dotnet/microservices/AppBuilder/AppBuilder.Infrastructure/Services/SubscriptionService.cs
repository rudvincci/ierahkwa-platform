using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.Extensions.Logging;

namespace AppBuilder.Infrastructure.Services;

/// <summary>Plans, subscriptions, build credits. IERAHKWA Appy: Free/Pro/Enterprise.</summary>
public class SubscriptionService : ISubscriptionService
{
    private static readonly List<Subscription> _subs = new();
    private static readonly List<SubscriptionPlan> _plans = new();
    private static readonly object _lock = new();
    private readonly IAuthService _auth;
    private readonly ILogger<SubscriptionService> _log;

    static SubscriptionService()
    {
        _plans.AddRange(new[]
        {
            new SubscriptionPlan { Id = "plan-free", Name = "Free", Tier = PlanTier.Free, PriceMonthly = 0, PriceYearly = 0, BuildCreditsPerMonth = 5, UnlimitedBuilds = false, IsActive = true, Features = new[] { "5 builds/month", "Android", "Browser preview", "QR code" } },
            new SubscriptionPlan { Id = "plan-pro", Name = "Pro", Tier = PlanTier.Pro, PriceMonthly = 19, PriceYearly = 190, BuildCreditsPerMonth = 50, UnlimitedBuilds = false, IsActive = true, Features = new[] { "50 builds/month", "All platforms", "Push notifications", "AI assistant", "Priority support" } },
            new SubscriptionPlan { Id = "plan-enterprise", Name = "Enterprise", Tier = PlanTier.Enterprise, PriceMonthly = 99, PriceYearly = 990, BuildCreditsPerMonth = 0, UnlimitedBuilds = true, IsActive = true, Features = new[] { "Unlimited builds", "WordPress plugin", "Dedicated support", "Custom branding", "SLA" } }
        });
    }

    public SubscriptionService(IAuthService auth, ILogger<SubscriptionService> log)
    {
        _auth = auth;
        _log = log;
    }

    public IReadOnlyList<SubscriptionPlan> GetPlans() => _plans.Where(p => p.IsActive).ToList();
    public SubscriptionPlan? GetPlanByTier(PlanTier tier) => _plans.FirstOrDefault(p => p.Tier == tier);
    public SubscriptionPlan? GetPlanById(string id) => _plans.FirstOrDefault(p => p.Id == id);

    public Subscription? GetUserSubscription(string userId)
    {
        lock (_lock) return _subs.FirstOrDefault(s => s.UserId == userId && s.Status == SubscriptionStatus.Active);
    }

    public bool HasBuildCredits(string userId)
    {
        var u = _auth.GetUserById(userId);
        return u != null && (u.BuildCredits > 0 || u.PlanTier == PlanTier.Enterprise);
    }

    public bool ConsumeBuildCredit(string userId)
    {
        var u = _auth.GetUserById(userId);
        if (u != null && u.PlanTier == PlanTier.Enterprise) return true; // unlimited
        return _auth.TryConsumeBuildCredit(userId);
    }

    public void GrantBuildCredits(string userId, int count) => _auth.GrantBuildCredits(userId, count);

    public void EnsureFreeTierDefaults(string userId)
    {
        var u = _auth.GetUserById(userId);
        if (u != null && u.PlanTier == PlanTier.Free && u.BuildCredits < 5)
            _auth.GrantBuildCredits(userId, 5 - u.BuildCredits);
    }

    public Subscription? Subscribe(string userId, string planId, PaymentMethod method)
    {
        lock (_lock)
        {
            var plan = _plans.FirstOrDefault(p => p.Id == planId);
            if (plan == null) return null;
            var sub = new Subscription
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                PlanId = planId,
                PaymentMethod = method,
                Status = SubscriptionStatus.Active,
                CurrentPeriodStart = DateTime.UtcNow,
                CurrentPeriodEnd = DateTime.UtcNow.AddMonths(1),
                CreatedAt = DateTime.UtcNow
            };
            _subs.Add(sub);
            var credits = plan.UnlimitedBuilds ? 9999 : plan.BuildCreditsPerMonth;
            _auth.GrantBuildCredits(userId, credits);
            _log.LogInformation("IERAHKWA Appy: User {UserId} subscribed to {Plan}", userId, plan.Name);
            return sub;
        }
    }
}
