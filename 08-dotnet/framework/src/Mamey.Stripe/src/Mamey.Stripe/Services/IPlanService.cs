//using Mamey.Stripe.Models;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface IPlanService
//    {
//        /// <summary>
//        /// Creates a new subscription plan.
//        /// </summary>
//        /// <param name="request">Parameters for the plan creation, including the associated product, pricing details, billing interval, and other options.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the create operation.</param>
//        /// <returns>The created Plan object.</returns>
//        Task<Plan> CreateAsync(PlanRequest request, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves details of an existing plan by its ID.
//        /// </summary>
//        /// <param name="planId">The unique identifier of the plan to retrieve.</param>
//        /// <returns>The requested Plan object.</returns>
//        Task<Plan> RetrieveAsync(string planId);

//        /// <summary>
//        /// Updates an existing plan with new information or options. Note that some attributes may not be updatable after creation.
//        /// </summary>
//        /// <param name="planId">The ID of the plan to update.</param>
//        /// <param name="updateRequest">Updated information for the plan.</param>
//        /// <returns>The updated Plan object.</returns>
//        Task<Plan> UpdateAsync(string planId, PlanUpdateRequest updateRequest);

//        /// <summary>
//        /// Lists all plans, optionally filtered by parameters such as product ID or active status.
//        /// </summary>
//        /// <param name="request">Parameters to filter the list of plans.</param>
//        /// <returns>A list of Plan objects.</returns>
//        Task<IEnumerable<Plan>> ListAsync(PlanListRequest request);

//        /// <summary>
//        /// Deletes a plan, making it inactive and unavailable for new subscriptions. Existing subscriptions are not affected.
//        /// </summary>
//        /// <param name="planId">The ID of the plan to delete.</param>
//        /// <returns>A boolean indicating the success of the deletion.</returns>
//        Task<bool> DeleteAsync(string planId);
//    }
//}
//public class PlanService : IPlanService
//    {
//        private readonly StripeClient _stripeClient;
//        private readonly ILogger<PlanService> _logger;

//        public PlanService(StripeClient stripeClient, ILogger<PlanService> logger)
//        {
//            _stripeClient = stripeClient ?? throw new ArgumentNullException(nameof(stripeClient));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task<Plan> CreateAsync(PlanRequest request, string idempotencyKey = null)
//        {
//            try
//            {
//                var options = new PlanCreateOptions
//                {
//                    Amount = request.Amount,
//                    Currency = request.Currency,
//                    Interval = request.Interval,
//                    Product = request.ProductId,
//                    // Additional fields from request
//                };
//                var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//                var service = new PlanService(_stripeClient);
//                var plan = await service.CreateAsync(options, requestOptions);
//                return plan; // Convert Stripe Plan to your Plan model
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating plan with IdempotencyKey {IdempotencyKey}", idempotencyKey);
//                throw new ApplicationException("Failed to create plan.", ex);
//            }
//        }

//        public async Task<Plan> RetrieveAsync(string planId)
//        {
//            try
//            {
//                var service = new PlanService(_stripeClient);
//                var plan = await service.GetAsync(planId);
//                return plan; // Convert Stripe Plan to your Plan model
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving plan {PlanId}", planId);
//                throw new ApplicationException($"Failed to retrieve plan with ID {planId}.", ex);
//            }
//        }

//        public async Task<Plan> UpdateAsync(string planId, PlanUpdateRequest updateRequest)
//        {
//            try
//            {
//                var options = new PlanUpdateOptions
//                {
//                    // Map updated fields
//                };
//                var service = new PlanService(_stripeClient);
//                var plan = await service.UpdateAsync(planId, options);
//                return plan; // Convert Stripe Plan to your Plan model
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating plan {PlanId}", planId);
//                throw new ApplicationException($"Failed to update plan with ID {planId}.", ex);
//            }
//        }

//        public async Task<IEnumerable<Plan>> ListAsync(PlanListRequest request)
//        {
//            try
//            {
//                var options = new PlanListOptions
//                {
//                    // Set list options based on request
//                };
//                var service = new PlanService(_stripeClient);
//                StripeList<Plan> plans = await service.ListAsync(options);
//                return plans; // Convert Stripe Plan list to your Plan model list
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error listing plans");
//                throw new ApplicationException("Failed to list plans.", ex);
//            }
//        }

//        public async Task<bool> DeleteAsync(string planId)
//        {
//            try
//            {
//                var service = new PlanService(_stripeClient);
//                var deleted = await service.DeleteAsync(planId);
//                return deleted.Deleted; // Check deletion status
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting plan {PlanId}", planId);
//                throw new ApplicationException($"Failed to delete plan with ID {planId}.", ex);
//            }
//        }
//    }