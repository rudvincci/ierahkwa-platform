//using Mamey.Stripe.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface IDisputeService
//    {
//        /// <summary>
//        /// Retrieves a dispute by its ID.
//        /// </summary>
//        /// <param name="disputeId">The unique identifier of the dispute to retrieve.</param>
//        /// <returns>The requested Dispute object.</returns>
//        Task<Dispute> RetrieveAsync(string disputeId);

//        /// <summary>
//        /// Updates a dispute, typically to submit evidence or to add more details.
//        /// </summary>
//        /// <param name="disputeId">The unique identifier of the dispute to update.</param>
//        /// <param name="updateRequest">Dispute update parameters, including evidence.</param>
//        /// <returns>The updated Dispute object.</returns>
//        Task<Dispute> UpdateAsync(string disputeId, DisputeUpdateRequest updateRequest);

//        /// <summary>
//        /// Lists all disputes, optionally filtered by parameters such as date range, status, or associated charge.
//        /// </summary>
//        /// <param name="request">Parameters to filter the list of disputes.</param>
//        /// <returns>A list of Dispute objects.</returns>
//        Task<IEnumerable<Dispute>> ListAsync(DisputeListRequest request);

//        /// <summary>
//        /// Closes a dispute, indicating that no further evidence will be submitted.
//        /// This action is final and cannot be undone.
//        /// </summary>
//        /// <param name="disputeId">The unique identifier of the dispute to close.</param>
//        /// <returns>The closed Dispute object.</returns>
//        Task<Dispute> CloseAsync(string disputeId);
//    }
//}
//public class DisputeService : IDisputeService
//    {
//        private readonly StripeApiClient _stripeClient;
//        private readonly ILogger<DisputeService> _logger;

//        public DisputeService(StripeApiClient stripeClient, ILogger<DisputeService> logger)
//        {
//            _stripeClient = stripeClient ?? throw new ArgumentNullException(nameof(stripeClient));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task<Dispute> RetrieveAsync(string disputeId)
//        {
//            try
//            {
//                return await _stripeClient.GetDisputeAsync(disputeId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to retrieve dispute {DisputeId}", disputeId);
//                throw;
//            }
//        }

//        public async Task<Dispute> UpdateAsync(string disputeId, DisputeUpdateRequest updateRequest)
//        {
//            try
//            {
//                return await _stripeClient.UpdateDisputeAsync(disputeId, updateRequest);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to update dispute {DisputeId}", disputeId);
//                throw;
//            }
//        }

//        public async Task<IEnumerable<Dispute>> ListAsync(DisputeListRequest request)
//        {
//            try
//            {
//                return await _stripeClient.ListDisputesAsync(request);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to list disputes");
//                throw;
//            }
//        }

//        public async Task<Dispute> CloseAsync(string disputeId)
//        {
//            try
//            {
//                return await _stripeClient.CloseDisputeAsync(disputeId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to close dispute {DisputeId}", disputeId);
//                throw;
//            }
//        }
//    }