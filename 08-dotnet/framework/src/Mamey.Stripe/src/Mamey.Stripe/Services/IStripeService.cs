using Mamey.Stripe.Configuration;
using Mamey.Stripe.Models;

namespace Mamey.Stripe.Services;

public interface IStripeService
{
    Task<string> CreatePaymentIntentAsync(PaymentIntentRequest request);
    Task<string> CreateSubscriptionAsync(SubscriptionRequest request);
    // Additional methods for refunds, customers, invoices, etc.
}
public class StripeServiceBase : IStripeService
{
    protected readonly StripeOptions _options;

    public StripeServiceBase(StripeOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        StripeConfiguration.ApiKey = _options.ApiKey;
    }

    public async Task<string> CreatePaymentIntentAsync(PaymentIntentRequest request)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(new PaymentIntentCreateOptions
            {
                Amount = Convert.ToInt64(request.Amount * 100), // Convert to smallest unit
                Currency = request.Currency,
            });

            return paymentIntent.Id;
        }
        catch (StripeException ex)
        {
            // Handle Stripe-specific exceptions here
            // Log the exception, throw a custom application exception, etc.
            throw new ApplicationException($"Failed to create payment intent: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            // Handle generic exceptions here
            throw new ApplicationException("An error occurred while creating payment intent.", ex);
        }
    }

    public async Task<string> CreateSubscriptionAsync(SubscriptionRequest request)
    {
        try
        {
            var service = new SubscriptionService();
            var subscription = await service.CreateAsync(new SubscriptionCreateOptions
            {
                Customer = request.CustomerId,
                Items = new List<SubscriptionItemOptions> { new SubscriptionItemOptions { Plan = request.PlanId } },
            });

            return subscription.Id;
        }
        catch (StripeException ex)
        {
            // Handle Stripe-specific exceptions here
            throw new ApplicationException($"Failed to create subscription: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            // Handle generic exceptions here
            throw new ApplicationException("An error occurred while creating subscription.", ex);
        }
    }
    public async Task HandleWebhookEventAsync(string jsonBody)
    {
        try
        {
            var stripeEvent = EventUtility.ParseEvent(jsonBody);
            // Handle different event types accordingly
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    // Handle payment intent succeeded
                    break;
                case "invoice.paid":
                    // Handle invoice paid
                    break;
                // Add more cases as needed
                default:
                    // Handle unknown event type
                    break;
            }
        }
        catch (StripeException ex)
        {
            throw new ApplicationException($"Failed to handle webhook event: {ex.Message}", ex);
        }
    }
    // Implementations for RefundRequest, CustomerRequest, InvoiceRequest, and handling WebhookEvent
}



