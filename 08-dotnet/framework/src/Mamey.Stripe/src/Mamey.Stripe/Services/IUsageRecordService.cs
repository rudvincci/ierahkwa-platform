//using Mamey.Stripe.Models;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface IUsageRecordService
//    {
//        /// <summary>
//        /// Creates a usage record for a specified subscription item, indicating the quantity used by the customer.
//        /// </summary>
//        /// <param name="subscriptionItemId">The ID of the subscription item to which the usage is being reported.</param>
//        /// <param name="quantity">The quantity of usage to report.</param>
//        /// <param name="timestamp">The timestamp when the usage occurred.</param>
//        /// <param name="action">Determines how the usage quantity is recorded. Can be 'increment' to add to the current quantity or 'set' to overwrite the existing quantity.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the usage record creation.</param>
//        /// <returns>The created UsageRecord object.</returns>
//        Task<UsageRecord> CreateAsync(string subscriptionItemId, int quantity, DateTime timestamp, string action, string idempotencyKey = null);
//    }
//}
//public class UsageRecordService : IUsageRecordService
//    {
//        private readonly ILogger<UsageRecordService> _logger;

//        public UsageRecordService(ILogger<UsageRecordService> logger)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task<UsageRecord> CreateAsync(string subscriptionItemId, int quantity, DateTime timestamp, string action, string idempotencyKey = null)
//        {
//            try
//            {
//                var options = new UsageRecordCreateOptions
//                {
//                    SubscriptionItem = subscriptionItemId,
//                    Quantity = quantity,
//                    Timestamp = timestamp,
//                    Action = action
//                };
//                var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//                var service = new UsageRecordService();
//                var usageRecord = await service.CreateAsync(options, requestOptions);
//                return new UsageRecord { /* Map Stripe UsageRecord to your UsageRecord model */ };
//            }
//            catch (StripeException ex)
//            {
//                _logger.LogError(ex, "Error creating usage record for SubscriptionItem {SubscriptionItemId} with IdempotencyKey {IdempotencyKey}", subscriptionItemId, idempotencyKey);
//                throw new ApplicationException($"Failed to create usage record for subscription item {subscriptionItemId}.", ex);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An unexpected error occurred while creating a usage record for SubscriptionItem {SubscriptionItemId}.", subscriptionItemId);
//                throw new ApplicationException($"An unexpected error occurred while creating a usage record for subscription item {subscriptionItemId}.", ex);
//            }
//        }
//    }