//using Mamey.Stripe.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface IRefundService
//    {
//        /// <summary>
//        /// Creates a refund for a specific charge or payment intent.
//        /// </summary>
//        /// <param name="request">Refund creation parameters, including amount and reason.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the create operation.</param>
//        /// <returns>The created Refund object.</returns>
//        Task<Refund> CreateAsync(RefundRequest request, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves a refund by its Stripe ID.
//        /// </summary>
//        /// <param name="refundId">The unique identifier of the refund to retrieve.</param>
//        /// <returns>The requested Refund object.</returns>
//        Task<Refund> RetrieveAsync(string refundId);

//        /// <summary>
//        /// Updates an existing refund with new metadata.
//        /// Useful for adding additional context or information after the refund has been created.
//        /// </summary>
//        /// <param name="refundId">The unique identifier of the refund to update.</param>
//        /// <param name="metadata">New metadata for the refund.</param>
//        /// <returns>The updated Refund object.</returns>
//        Task<Refund> UpdateAsync(string refundId, Dictionary<string, string> metadata);

//        /// <summary>
//        /// Lists all refunds, optionally filtered by parameters such as charge ID or created date.
//        /// </summary>
//        /// <param name="request">Parameters to filter the list of refunds.</param>
//        /// <returns>A list of Refund objects.</returns>
//        Task<IEnumerable<Refund>> ListAsync(RefundListRequest request);

//        /// <summary>
//        /// Cancels a pending refund. This is applicable only under specific conditions determined by the payment processor.
//        /// </summary>
//        /// <param name="refundId">The unique identifier of the refund to cancel.</param>
//        /// <returns>A boolean indicating the success of the cancellation.</returns>
//        Task<bool> CancelAsync(string refundId);
//    }
//}
//public class RefundService : IRefundService
//    {
//        private readonly StripeClient _stripeClient;
//        private readonly ILogger<RefundService> _logger;

//        public RefundService(StripeClient stripeClient, ILogger<RefundService> logger)
//        {
//            _stripeClient = stripeClient ?? throw new ArgumentNullException(nameof(stripeClient));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task<Refund> CreateAsync(RefundRequest request, string idempotencyKey = null)
//        {
//            try
//            {
//                var options = new RefundCreateOptions
//                {
//                    Charge = request.ChargeId,
//                    Amount = request.Amount,
//                    Reason = request.Reason,
//                    Metadata = request.Metadata,
//                };
//                var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//                var service = new RefundService(_stripeClient);
//                var refund = await service.CreateAsync(options, requestOptions);
//                return refund; // Convert Stripe Refund to your Refund model
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating refund with IdempotencyKey {IdempotencyKey}", idempotencyKey);
//                throw new ApplicationException("Failed to create refund.", ex);
//            }
//        }

//        public async Task<Refund> RetrieveAsync(string refundId)
//        {
//            try
//            {
//                var service = new RefundService(_stripeClient);
//                var refund = await service.GetAsync(refundId);
//                return refund; // Convert Stripe Refund to your Refund model
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving refund {RefundId}", refundId);
//                throw new ApplicationException($"Failed to retrieve refund with ID {refundId}.", ex);
//            }
//        }

//        public async Task<Refund> UpdateAsync(string refundId, Dictionary<string, string> metadata)
//        {
//            try
//            {
//                var options = new RefundUpdateOptions
//                {
//                    Metadata = metadata,
//                };
//                var service = new RefundService(_stripeClient);
//                var refund = await service.UpdateAsync(refundId, options);
//                return refund; // Convert Stripe Refund to your Refund model
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating refund {RefundId}", refundId);
//                throw new ApplicationException($"Failed to update refund with ID {refundId}.", ex);
//            }
//        }

//        public async Task<IEnumerable<Refund>> ListAsync(RefundListRequest request)
//        {
//            try
//            {
//                var options = new RefundListOptions
//                {
//                    Charge = request.ChargeId,
//                    Limit = request.Limit,
//                };
//                var service = new RefundService(_stripeClient);
//                StripeList<Refund> refunds = await service.ListAsync(options);
//                return refunds; // Convert Stripe Refund list to your Refund model list
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error listing refunds");
//                throw new ApplicationException("Failed to list refunds.", ex);
//            }
//        }

//        public async Task<bool> CancelAsync(string refundId)
//        {
//            try
//            {
//                // Note: Stripe API does not support refund cancellation directly. 
//                // This method would need custom logic based on your application's needs.
//                throw new NotSupportedException("Canceling refunds is not supported directly by Stripe.");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error canceling refund {RefundId}", refundId);
//                throw new ApplicationException($"Failed to cancel refund with ID {refundId}.", ex);
//            }
//        }
//    }