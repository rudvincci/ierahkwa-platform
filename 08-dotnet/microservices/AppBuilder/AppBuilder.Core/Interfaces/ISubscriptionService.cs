using AppBuilder.Core.Models;

namespace AppBuilder.Core.Interfaces;

/// <summary>Subscriptions, plans, build credits. Appy: Free/Pro/Enterprise, Build Credits, Premium Features.</summary>
public interface ISubscriptionService
{
    IReadOnlyList<SubscriptionPlan> GetPlans();
    SubscriptionPlan? GetPlanById(string id);
    SubscriptionPlan? GetPlanByTier(PlanTier tier);
    Subscription? GetUserSubscription(string userId);
    bool HasBuildCredits(string userId);
    bool ConsumeBuildCredit(string userId);
    void GrantBuildCredits(string userId, int count);
    Subscription? Subscribe(string userId, string planId, PaymentMethod method);
    void EnsureFreeTierDefaults(string userId);
}
