//using Mamey.Stripe.Models;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface IWebhookService
//    {
//        /// <summary>
//        /// Processes incoming webhook events from Stripe.
//        /// </summary>
//        /// <param name="jsonPayload">The raw JSON payload received from Stripe.</param>
//        /// <param name="signatureHeader">The signature header provided by Stripe to verify the request.</param>
//        /// <param name="webhookSecret">The secret used to verify webhook signatures for security.</param>
//        /// <returns>A task indicating the processing outcome.</returns>
//        Task<WebhookEventResult> ProcessEventAsync(string jsonPayload, string signatureHeader, string webhookSecret);
//    }

//    public class WebhookEventResult
//    {
//        public bool Success { get; set; }
//        public string ProcessedEventId { get; set; }
//        public string ErrorMessage { get; set; }
//        // Additional fields as needed for detailed processing results.
//    }
//}
//public class WebhookService : IWebhookService
//{
//    private readonly ILogger<WebhookService> _logger;

//    public WebhookService(ILogger<WebhookService> logger)
//    {
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//    }

//    public async Task<WebhookEventResult> ProcessEventAsync(string jsonPayload, string signatureHeader, string webhookSecret)
//    {
//        try
//        {
//            // Verify the event by comparing its computed signature with the signature sent in the header
//            var eventStriped = EventUtility.ConstructEvent(jsonPayload, signatureHeader, webhookSecret);
            
//            // Process the event based on its type
//            switch (eventStriped.Type)
//            {
//                case "payment_intent.succeeded":
//                    // Handle payment intent succeeded
//                    break;
//                case "charge.refunded":
//                    // Handle charge refunded
//                    break;
//                // Add more case statements as needed for other event types
//                default:
//                    _logger.LogInformation("Unhandled event type: {EventType}", eventStriped.Type);
//                    break;
//            }

//            return new WebhookEventResult { Success = true, ProcessedEventId = eventStriped.Id };
//        }
//        catch (StripeException ex)
//        {
//            _logger.LogError(ex, "StripeException occurred while processing webhook event.");
//            return new WebhookEventResult { Success = false, ErrorMessage = $"StripeException: {ex.Message}" };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "An error occurred while processing webhook event.");
//            return new WebhookEventResult { Success = false, ErrorMessage = $"Unhandled exception: {ex.Message}" };
//        }
//    }
//}