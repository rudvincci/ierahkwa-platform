//using Mamey.Stripe.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface IProductService
//    {
//        /// <summary>
//        /// Creates a new product within Stripe.
//        /// </summary>
//        /// <param name="request">The product creation parameters.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the create operation.</param>
//        /// <returns>The created Product object.</returns>
//        Task<Product> CreateAsync(ProductRequest request, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves a product by its Stripe ID.
//        /// </summary>
//        /// <param name="productId">The unique identifier of the product to retrieve.</param>
//        /// <returns>The requested Product object.</returns>
//        Task<Product> RetrieveAsync(string productId);

//        /// <summary>
//        /// Updates an existing product with new information.
//        /// </summary>
//        /// <param name="productId">The unique identifier of the product to update.</param>
//        /// <param name="request">Product update parameters.</param>
//        /// <returns>The updated Product object.</returns>
//        Task<Product> UpdateAsync(string productId, ProductUpdateRequest request);

//        /// <summary>
//        /// Deletes a product from Stripe. Note that deleting a product is irreversible.
//        /// </summary>
//        /// <param name="productId">The unique identifier of the product to delete.</param>
//        /// <returns>A boolean indicating the success of the operation.</returns>
//        Task<bool> DeleteAsync(string productId);

//        /// <summary>
//        /// Lists all products, optionally filtered by parameters.
//        /// </summary>
//        /// <param name="request">Parameters to filter the list of products.</param>
//        /// <returns>A list of Product objects.</returns>
//        Task<IEnumerable<Product>> ListAsync(ProductListRequest request);

//        /// <summary>
//        /// Archives a product, making it inactive and unavailable for new purchases or subscriptions.
//        /// Archived products can be reactivated.
//        /// </summary>
//        /// <param name="productId">The unique identifier of the product to archive.</param>
//        /// <returns>The archived Product object.</returns>
//        Task<Product> ArchiveAsync(string productId);

//        /// <summary>
//        /// Reactivates an archived product, making it available for new purchases or subscriptions.
//        /// </summary>
//        /// <param name="productId">The unique identifier of the product to reactivate.</param>
//        /// <returns>The reactivated Product object.</returns>
//        Task<Product> ReactivateAsync(string productId);
//    }
//}
//public class ProductService : IProductService
//    {
//        private readonly StripeClient _stripeClient;
//        private readonly ILogger<ProductService> _logger;

//        public ProductService(StripeClient stripeClient, ILogger<ProductService> logger)
//        {
//            _stripeClient = stripeClient ?? throw new ArgumentNullException(nameof(stripeClient));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task<Product> CreateAsync(ProductRequest request, string idempotencyKey = null)
//        {
//            try
//            {
//                var options = new ProductCreateOptions
//                {
//                    Name = request.Name,
//                    Description = request.Description,
//                    // Map other fields from request to options as needed
//                };
//                var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//                var service = new ProductService(_stripeClient);
//                var product = await service.CreateAsync(options, requestOptions);
//                return product; // Convert Stripe Product to your Product model
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating product with IdempotencyKey {IdempotencyKey}", idempotencyKey);
//                throw new ApplicationException("Failed to create product.", ex);
//            }
//        }

//        public async Task<Product> RetrieveAsync(string productId)
//        {
//            try
//            {
//                var service = new ProductService(_stripeClient);
//                var product = await service.GetAsync(productId);
//                return product; // Convert Stripe Product to your Product model
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving product {ProductId}", productId);
//                throw new ApplicationException($"Failed to retrieve product with ID {productId}.", ex);
//            }
//        }

//        public async Task<Product> UpdateAsync(string productId, ProductUpdateRequest request)
//        {
//            try
//            {
//                var options = new ProductUpdateOptions
//                {
//                    Name = request.Name,
//                    Description = request.Description,
//                    // Map other updatable fields from request to options
//                };
//                var service = new ProductService(_stripeClient);
//                var product = await service.UpdateAsync(productId, options);
//                return product; // Convert Stripe Product to your Product model
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating product {ProductId}", productId);
//                throw new ApplicationException($"Failed to update product with ID {productId}.", ex);
//            }
//        }

//        public async Task<bool> DeleteAsync(string productId)
//        {
//            try
//            {
//                var service = new ProductService(_stripeClient);
//                var deleted = await service.DeleteAsync(productId);
//                return deleted.Deleted;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting product {ProductId}", productId);
//                throw new ApplicationException($"Failed to delete product with ID {productId}.", ex);
//            }
//        }

//        public async Task<IEnumerable<Product>> ListAsync(ProductListRequest request)
//        {
//            try
//            {
//                var options = new ProductListOptions
//                {
//                    Active = request.Active,
//                    // Additional filters can be applied here
//                };
//                var service = new ProductService(_stripeClient);
//                StripeList<Product> products = await service.ListAsync(options);
//                return products; // Convert Stripe Product list to your Product model list
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error listing products");
//                throw new ApplicationException("Failed to list products.", ex);
//            }
//        }

//        public async Task<Product> ArchiveAsync(string productId)
//        {
//            // Assuming Stripe's Product service supports an "archive" operation, 
//            // or implement custom logic to mark a product as archived in your database.
//            throw new NotImplementedException();
//        }

//        public async Task<Product> ReactivateAsync(string productId)
//        {
//            // Assuming Stripe's Product service supports a "reactivate" operation, 
//            // or implement custom logic to mark a product as active in your database.
//            throw new NotImplementedException();
//        }
//    }