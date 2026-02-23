namespace Mamey.Stripe.Utilities;

public class StripeMapper
{
    // Method to convert from Stripe's customer object to CustomerRequest model
    // Similar methods for other mappings
}
public class WebhookValidator
{
    private readonly string _webhookSecret;

    public WebhookValidator(string webhookSecret)
    {
        _webhookSecret = webhookSecret ?? throw new ArgumentNullException(nameof(webhookSecret));
    }

    /// <summary>
    /// Validates the Stripe webhook event by verifying its signature.
    /// </summary>
    /// <param name="json">The raw JSON payload of the webhook event.</param>
    /// <param name="stripeSignatureHeader">The value of the 'Stripe-Signature' header from the webhook request.</param>
    /// <returns>The Stripe.Event if valid, otherwise null.</returns>
    public Event ValidateWebhook(string json, string stripeSignatureHeader)
    {
        try
        {
            // Throws an exception if the event cannot be validated
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignatureHeader,
                _webhookSecret
            );

            return stripeEvent;
        }
        catch (StripeException ex)
        {
            // Log the exception for debugging purposes
            Console.WriteLine($"Webhook error: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            // Handle other unexpected errors
            Console.WriteLine($"General error in webhook validation: {ex.Message}");
            return null;
        }
    }
}