//namespace Mamey.Stripe.Services;

//public interface ISubscriptionService
//    {
//        /// <summary>
//        /// Creates a new subscription.
//        /// </summary>
//        /// <param name="request">Subscription creation parameters.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency.</param>
//        /// <returns>The created Subscription object.</returns>
//        Task<Subscription> CreateAsync(SubscriptionRequest request, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves a subscription by its ID.
//        /// </summary>
//        /// <param name="subscriptionId">The ID of the subscription to retrieve.</param>
//        /// <returns>The requested Subscription object.</returns>
//        Task<Subscription> RetrieveAsync(string subscriptionId);

//        /// <summary>
//        /// Updates an existing subscription.
//        /// </summary>
//        /// <param name="subscriptionId">The ID of the subscription to update.</param>
//        /// <param name="request">Subscription update parameters.</param>
//        /// <returns>The updated Subscription object.</returns>
//        Task<Subscription> UpdateAsync(string subscriptionId, SubscriptionUpdateRequest request);

//        /// <summary>
//        /// Cancels a subscription.
//        /// </summary>
//        /// <param name="subscriptionId">The ID of the subscription to cancel.</param>
//        /// <param name="cancelAtPeriodEnd">Determines if the subscription should be canceled immediately or at the end of the billing period.</param>
//        /// <returns>The canceled Subscription object.</returns>
//        Task<Subscription> CancelAsync(string subscriptionId, bool cancelAtPeriodEnd = false);

//        /// <summary>
//        /// Lists all subscriptions, optionally filtered by parameters.
//        /// </summary>
//        /// <param name="request">Parameters to filter the list of subscriptions.</param>
//        /// <returns>A list of Subscription objects.</returns>
//        Task<IEnumerable<Subscription>> ListAsync(SubscriptionListRequest request);

//        /// <summary>
//        /// Handles payment failures for subscriptions, such as updating payment methods or retrying payments.
//        /// </summary>
//        /// <param name="subscriptionId">The ID of the subscription with a payment failure.</param>
//        /// <returns>Information on how to resolve the payment failure.</returns>
//        Task<SubscriptionPaymentFailureResult> HandlePaymentFailureAsync(string subscriptionId);

//        /// <summary>
//        /// Pauses a subscription.
//        /// </summary>
//        /// <param name="subscriptionId">The ID of the subscription to pause.</param>
//        /// <returns>The paused Subscription object.</returns>
//        Task<Subscription> PauseAsync(string subscriptionId);

//        /// <summary>
//        /// Resumes a paused subscription.
//        /// </summary>
//        /// <param name="subscriptionId">The ID of the paused subscription to resume.</param>
//        /// <returns>The resumed Subscription object.</returns>
//        Task<Subscription> ResumeAsync(string subscriptionId);
//    }

//    public class SubscriptionPaymentFailureResult
//    {
//        public bool RetryRecommended { get; set; }
//        public DateTime? NextRetryDate { get; set; }
//        public string SuggestedAction { get; set; }
//        // Additional properties and suggestions based on the failure scenario
//    }
//    public class SubscriptionService : ISubscriptionService
//    {
//        public async Task<Subscription> CreateAsync(SubscriptionRequest request, string idempotencyKey = null)
//        {
//            try
//            {
//                var options = new SubscriptionCreateOptions
//                {
//                    Customer = request.CustomerId,
//                    Items = request.Items,
//                    Expand = new List<string> { "latest_invoice.payment_intent" },
//                    Metadata = request.Metadata
//                };
//                var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//                var service = new SubscriptionService();
//                var subscription = await service.CreateAsync(options, requestOptions);
//                return subscription; // Convert to your Subscription model
//            }
//            catch (StripeException ex)
//            {
//                // Handle Stripe-specific exceptions here
//                throw new ApplicationException($"Failed to create subscription: {ex.Message}", ex);
//            }
//        }

//        public async Task<Subscription> RetrieveAsync(string subscriptionId)
//        {
//            try
//            {
//                var service = new SubscriptionService();
//                var subscription = await service.GetAsync(subscriptionId);
//                return subscription; // Convert to your Subscription model
//            }
//            catch (StripeException ex)
//            {
//                throw new ApplicationException($"Failed to retrieve subscription {subscriptionId}: {ex.Message}", ex);
//            }
//        }

//        public async Task<Subscription> UpdateAsync(string subscriptionId, SubscriptionUpdateRequest request)
//        {
//            try
//            {
//                var options = new SubscriptionUpdateOptions
//                {
//                    // Configure update options based on SubscriptionUpdateRequest
//                };
//                var service = new SubscriptionService();
//                var subscription = await service.UpdateAsync(subscriptionId, options);
//                return subscription; // Convert to your Subscription model
//            }
//            catch (StripeException ex)
//            {
//                throw new ApplicationException($"Failed to update subscription {subscriptionId}: {ex.Message}", ex);
//            }
//        }

//        public async Task<Subscription> CancelAsync(string subscriptionId, bool cancelAtPeriodEnd = false)
//        {
//            try
//            {
//                var options = new SubscriptionCancelOptions { InvoiceNow = !cancelAtPeriodEnd, Prorate = cancelAtPeriodEnd };
//                var service = new SubscriptionService();
//                var subscription = await service.CancelAsync(subscriptionId, options);
//                return subscription; // Convert to your Subscription model
//            }
//            catch (StripeException ex)
//            {
//                throw new ApplicationException($"Failed to cancel subscription {subscriptionId}: {ex.Message}", ex);
//            }
//        }

//        public async Task<IEnumerable<Subscription>> ListAsync(SubscriptionListRequest request)
//        {
//            try
//            {
//                var options = new SubscriptionListOptions
//                {
//                    Customer = request.CustomerId,
//                    Status = request.Status,
//                    Limit = request.Limit
//                };
//                var service = new SubscriptionService();
//                StripeList<Subscription> subscriptions = await service.ListAsync(options);
//                return subscriptions; // Convert to your list of Subscription models
//            }
//            catch (StripeException ex)
//            {
//                throw new ApplicationException("Failed to list subscriptions: " + ex.Message, ex);
//            }
//        }

//        public async Task<SubscriptionPaymentFailureResult> HandlePaymentFailureAsync(string subscriptionId)
//        {
//            // Implement logic to handle payment failures for subscriptions
//            throw new NotImplementedException();
//        }

//        public async Task<Subscription> PauseAsync(string subscriptionId)
//        {
//            // Implement logic to pause a subscription
//            throw new NotImplementedException();
//        }

//        public async Task<Subscription> ResumeAsync(string subscriptionId)
//        {
//            // Implement logic to resume a paused subscription
//            throw new NotImplementedException();
//        }
//    }