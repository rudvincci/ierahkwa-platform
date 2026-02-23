//using Mamey.Stripe.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface ITaxRateService
//    {
//        /// <summary>
//        /// Creates a new tax rate.
//        /// </summary>
//        /// <param name="request">Parameters for the tax rate creation, including the percentage and jurisdiction.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the create operation.</param>
//        /// <returns>The created TaxRate object.</returns>
//        Task<TaxRate> CreateAsync(TaxRateRequest request, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves details of an existing tax rate by its ID.
//        /// </summary>
//        /// <param name="taxRateId">The unique identifier of the tax rate to retrieve.</param>
//        /// <returns>The requested TaxRate object.</returns>
//        Task<TaxRate> RetrieveAsync(string taxRateId);

//        /// <summary>
//        /// Updates an existing tax rate with new information or metadata.
//        /// </summary>
//        /// <param name="taxRateId">The unique identifier of the tax rate to update.</param>
//        /// <param name="updateRequest">Tax rate update parameters.</param>
//        /// <returns>The updated TaxRate object.</returns>
//        Task<TaxRate> UpdateAsync(string taxRateId, TaxRateUpdateRequest updateRequest);

//        /// <summary>
//        /// Lists all tax rates, optionally filtered by parameters such as active status or country.
//        /// </summary>
//        /// <param name="request">Parameters to filter the list of tax rates.</param>
//        /// <returns>A list of TaxRate objects.</returns>
//        Task<IEnumerable<TaxRate>> ListAsync(TaxRateListRequest request);
//    }
//}
//public class TaxRateService : ITaxRateService
//    {
//        private readonly ILogger<TaxRateService> _logger;

//        public TaxRateService(ILogger<TaxRateService> logger)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task<TaxRate> CreateAsync(TaxRateRequest request, string idempotencyKey = null)
//        {
//            try
//            {
//                var options = new TaxRateCreateOptions
//                {
//                    DisplayName = request.DisplayName,
//                    Description = request.Description,
//                    Jurisdiction = request.Jurisdiction,
//                    Percentage = request.Percentage,
//                    Inclusive = request.Inclusive,
//                };
//                var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//                var service = new TaxRateService();
//                var taxRate = await service.CreateAsync(options, requestOptions);
//                return taxRate; // Convert Stripe TaxRate to your TaxRate model
//            }
//            catch (StripeException ex)
//            {
//                _logger.LogError(ex, "Error creating tax rate with IdempotencyKey {IdempotencyKey}", idempotencyKey);
//                throw new ApplicationException("Failed to create tax rate.", ex);
//            }
//        }

//        public async Task<TaxRate> RetrieveAsync(string taxRateId)
//        {
//            try
//            {
//                var service = new TaxRateService();
//                var taxRate = await service.GetAsync(taxRateId);
//                return taxRate; // Convert Stripe TaxRate to your TaxRate model
//            }
//            catch (StripeException ex)
//            {
//                _logger.LogError(ex, "Error retrieving tax rate {TaxRateId}", taxRateId);
//                throw new ApplicationException($"Failed to retrieve tax rate with ID {taxRateId}.", ex);
//            }
//        }

//        public async Task<TaxRate> UpdateAsync(string taxRateId, TaxRateUpdateRequest updateRequest)
//        {
//            try
//            {
//                var options = new TaxRateUpdateOptions
//                {
//                    // Map updated fields from updateRequest to options
//                };
//                var service = new TaxRateService();
//                var taxRate = await service.UpdateAsync(taxRateId, options);
//                return taxRate; // Convert Stripe TaxRate to your TaxRate model
//            }
//            catch (StripeException ex)
//            {
//                _logger.LogError(ex, "Error updating tax rate {TaxRateId}", taxRateId);
//                throw new ApplicationException($"Failed to update tax rate with ID {taxRateId}.", ex);
//            }
//        }

//        public async Task<IEnumerable<TaxRate>> ListAsync(TaxRateListRequest request)
//        {
//            try
//            {
//                var options = new TaxRateListOptions
//                {
//                    Active = request.Active,
//                    Inclusive = request.Inclusive,
//                    Country = request.Country,
//                    // Additional filters as needed
//                };
//                var service = new TaxRateService();
//                StripeList<TaxRate> taxRates = await service.ListAsync(options);
//                return taxRates; // Convert Stripe TaxRate list to your TaxRate model list
//            }
//            catch (StripeException ex)
//            {
//                _logger.LogError(ex, "Error listing tax rates");
//                throw new ApplicationException("Failed to list tax rates.", ex);
//            }
//        }
//    }