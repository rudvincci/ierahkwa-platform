namespace Mamey.Stripe.Models;

public class WebhookEvent
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string ObjectJson { get; set; } // Raw JSON object of the event
    // Additional fields for processing webhook events as needed
}
