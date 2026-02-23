namespace Mamey.Stripe.Models;

public class SubscriptionItem
{
    public string PlanId { get; set; }
    public int Quantity { get; set; }
    // Additional properties per the Stripe documentation
}
