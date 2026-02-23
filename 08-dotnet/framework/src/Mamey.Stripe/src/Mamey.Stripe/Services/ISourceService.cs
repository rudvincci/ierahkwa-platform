//using Mamey.Stripe.Models;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface ISourceService
//    {
//        /// <summary>
//        /// Creates a new payment source based on the specified parameters.
//        /// This can include creating sources for card payments, bank transfers, and more.
//        /// </summary>
//        /// <param name="request">Parameters for the source creation, including type and specific attributes.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the create operation.</param>
//        /// <returns>The created Source object.</returns>
//        Task<Source> CreateAsync(SourceRequest request, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves details of an existing payment source by its ID.
//        /// </summary>
//        /// <param name="sourceId">The unique identifier of the source to retrieve.</param>
//        /// <returns>The requested Source object.</returns>
//        Task<Source> RetrieveAsync(string sourceId);

//        /// <summary>
//        /// Updates an existing payment source with new information or metadata.
//        /// This might be used to attach additional details or update the source's status.
//        /// </summary>
//        /// <param name="sourceId">The unique identifier of the source to update.</param>
//        /// <param name="updateRequest">Source update parameters.</param>
//        /// <returns>The updated Source object.</returns>
//        Task<Source> UpdateAsync(string sourceId, SourceUpdateRequest updateRequest);

//        /// <summary>
//        /// Lists all payment sources associated with a customer.
//        /// </summary>
//        /// <param name="customerId">The Stripe customer ID whose sources are to be listed.</param>
//        /// <returns>A list of Source objects.</returns>
//        Task<IEnumerable<Source>> ListAsync(string customerId);
//    }
//}
//public class SourceService : ISourceService
//{
//    private readonly ILogger<SourceService> _logger;

//    public SourceService(ILogger<SourceService> logger)
//    {
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//    }

//    public async Task<Source> CreateAsync(SourceRequest request, string idempotencyKey = null)
//    {
//        try
//        {
//            var options = new SourceCreateOptions
//            {
//                Type = request.Type,
//                Currency = request.Currency,
//                Amount = request.Amount,
//                Metadata = request.Metadata,
//                // Additional attributes based on the type of source
//            };
//            var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//            var service = new SourceService();
//            var source = await service.CreateAsync(options, requestOptions);
//            return source; // Convert Stripe Source to your Source model
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error creating payment source with IdempotencyKey {IdempotencyKey}", idempotencyKey);
//            throw new ApplicationException("Failed to create payment source.", ex);
//        }
//    }

//    public async Task<Source> RetrieveAsync(string sourceId)
//    {
//        try
//        {
//            var service = new SourceService();
//            var source = await service.GetAsync(sourceId);
//            return source; // Convert Stripe Source to your Source model
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error retrieving payment source {SourceId}", sourceId);
//            throw new ApplicationException($"Failed to retrieve payment source with ID {sourceId}.", ex);
//        }
//    }

//    public async Task<Source> UpdateAsync(string sourceId, SourceUpdateRequest updateRequest)
//    {
//        try
//        {
//            var options = new SourceUpdateOptions
//            {
//                Metadata = updateRequest.Metadata,
//                // Map other updatable fields
//            };
//            var service = new SourceService();
//            var source = await service.UpdateAsync(sourceId, options);
//            return source; // Convert Stripe Source to your Source model
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error updating payment source {SourceId}", sourceId);
//            throw new ApplicationException($"Failed to update payment source with ID {sourceId}.", ex);
//        }
//    }

//    public async Task<IEnumerable<Source>> ListAsync(string customerId)
//    {
//        try
//        {
//            var options = new SourceListOptions
//            {
//                Customer = customerId,
//                // Additional filters as needed
//            };
//            var service = new SourceService();
//            StripeList<Source> sources = await service.ListAsync(options);
//            return sources; // Convert Stripe Source list to your Source model list
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error listing payment sources for customer {CustomerId}", customerId);
//            throw new ApplicationException($"Failed to list payment sources for customer ID {customerId}.", ex);
//        }
//    }
//}