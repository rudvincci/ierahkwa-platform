using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Products.Core.Repositories;
using Mamey.ApplicationName.Modules.Products.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;
using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Mamey.Bank.Shared.Types;
using Mamey.Constants.User;
using Mamey.Exceptions;
using Microsoft.Extensions.Logging;
using AccountCategory = Mamey.Bank.Shared.Types.AccountCategory;

namespace Mamey.ApplicationName.Modules.Products.Core.Services
{
    internal sealed class BankingProductService : IBankingProductService
    {
        private readonly ILogger<BankingProductService> _logger;
        private readonly IBankingProductRepository _repository;
        private readonly IMessageBroker _messageBroker;

        public BankingProductService(ILogger<BankingProductService> logger, 
            IBankingProductRepository repository, IMessageBroker messageBroker)
        {
            _logger = logger;
            _repository = repository;
            _messageBroker = messageBroker;
        }

        // public async Task<IEnumerable<ProductDto>> BrowseAsync(CancellationToken cancellationToken  = default)
        // {
        //     var entities = await _repository.BrowseAsync();
        //     return entities?.Select(e => e.AsDto());
        // }

        // public async Task<ProductDto> GetAsync(Guid productId, CancellationToken cancellationToken  = default)
        // {
        //     var entity = await _repository.GetAsync(productId);
        //     return entity?.AsDto();
        // }

        // public async Task CreateAsync(ProductDto product, CancellationToken cancellationToken  = default)
        // {
        //     var alreadyExists = await _repository.ExistsAsync(product.Id);
        //     if (alreadyExists)
        //     {
        //         throw new ProductAlreadyExistsException(product.Id);
        //     }

        //     await _repository.AddAsync(product.AsEntity());
        //     await _messageBroker.PublishAsync(new ProductCreated(product.Id));
        // }

        // public async Task UpdateAsync(ProductDto product, CancellationToken cancellationToken  = default)
        // {
        //     var exists = await _repository.ExistsAsync(product.Id);

        //     if (!exists)
        //     {
        //         throw new ProductNotFoundException(product.Id);
        //     }
            
        //     await _repository.UpdateAsync(product.AsEntity());
        // }


        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid productId)
        {
            return await _repository.GetByIdAsync(productId);
        }
        public async Task<Product> GetProductByNameAsync(string name)
        {
            return await _repository.GetByNameAsync(name);
        }

        public async Task AddProductAsync(Product product)
        {
            _logger.LogInformation("Adding a new banking product: {Name}", product.Name);
            await _repository.AddAsync(product);
            await _messageBroker.PublishAsync(new BankingProductCreated(product.Id));
            _logger.LogInformation("Successfully added product: {Name}", product.Name);
        }

        public async Task UpdateProductAsync(Product product)
        {
            _logger.LogInformation("Updating a new banking product: {Name}", product.Name);
            var existingProduct = await _repository.GetByIdAsync(product.Id);
            if (existingProduct != null)
            {
                product.Version = existingProduct.Version + 1;
                await _repository.UpdateAsync(product);
                await _messageBroker.PublishAsync(new BankingProductUpdated(product.Id));
                _logger.LogInformation($"Successfully updated product: {product.Name}" );
            }
            else
            {
                throw new MameyException($"Banking product with Id: '{product.Id}' does not exist.");
            }
        }

        public async Task DeleteProductAsync(Guid productId)
        {
            var existingProduct = await _repository.GetByIdAsync(productId);
            if (existingProduct is null)
            {
                throw new MameyException("Product with Id: '" + productId + "' does not exist.");
            }
            existingProduct.IsDeleted = true;
            await _repository.UpdateAsync(existingProduct);
            await _messageBroker.PublishAsync(new BankingProductDeleted(existingProduct.Id));
            
        }

        public async Task<IEnumerable<Benefit>> GetBenefitsByProductIdAsync(Guid productId)
        {
            return await _repository.GetBenefitsByProductIdAsync(productId);
        }

        public async Task<IEnumerable<Fee>> GetFeesByProductIdAsync(Guid productId)
        {
            return await _repository.GetFeesByProductIdAsync(productId);
        }
        public async Task<IEnumerable<Product>> SearchProductsAsync(
            AccountType? productType = null,
            string currency = null,
            AccountCategory? accountCategory = null)
        {
            var products = await _repository.GetAllAsync();

            if (productType.HasValue)
                products = products.Where(p => p.ProductType == productType.Value);

            if (!string.IsNullOrEmpty(currency))
                products = products.Where(p => p.Currency.Code.Equals(currency, StringComparison.OrdinalIgnoreCase));

            if (accountCategory.HasValue)
                products = products.Where(p => p.AccountCategory == accountCategory.Value);

            return products;
        }
        public async Task AddProductsAsync(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                await _repository.AddAsync(product);
            }
        }
        //
        public async Task<IEnumerable<Product>> GetProductsByAccountCategoryAsync(AccountCategory accountCategory)
        {
            return (await _repository.GetAllAsync())
                .Where(p => p.AccountCategory == accountCategory)
                .ToList();
        }
        /// <summary>
        /// Fetch all products that are currently active.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            var activeProducts = await _repository.GetAllAsync();
            return (activeProducts)
                .Where(p => p.Status.Equals(ProductStatus.Active))
                .ToList();
        }
        /// <summary>
        /// Retrieve all products of a specific type (e.g., Savings Account, Loan).
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductsByTypeAsync(AccountType productType)
        {
            return (await _repository.GetAllAsync())
                .Where(p => p.ProductType == productType)
                .ToList();
        }
        /// <summary>
        /// Retrieve all products that support a specific currency.
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductsByCurrencyAsync(string currency)
        {
            return (await _repository.GetAllAsync())
                .Where(p => p.Currency.Code.Equals(currency, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        /// <summary>
        /// Fetch all products that offer a specific benefit (e.g., Cashback, Insurance).
        /// </summary>
        /// <param name="benefitType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductsByBenefitAsync(string benefitType)
        {
            return (await _repository.GetAllAsync())
                .Where(p => p.Benefits.Any(b => b.BenefitType.Equals(benefitType, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
        /// <summary>
        /// Retrieve all products with fees above a certain amount.
        /// </summary>
        /// <param name="feeThreshold"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductsWithFeesAboveAsync(decimal feeThreshold)
        {
            return (await _repository.GetAllAsync())
                .Where(p => p.Fees.Any(f => f.Amount > feeThreshold))
                .ToList();
        }
        /// <summary>
        /// Fetch all products with tax rates falling within a specified range.
        /// </summary>
        /// <param name="minTaxRate"></param>
        /// <param name="maxTaxRate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductsByTaxRateRangeAsync(decimal minTaxRate, decimal maxTaxRate)
        {
            return (await _repository.GetAllAsync())
                .Where(p => p.ApplicableTaxes.Any(t => t.TaxRate >= minTaxRate && t.TaxRate <= maxTaxRate))
                .ToList();
        }
        /// <summary>
        /// Retrieve all products that match specific eligibility criteria (e.g., MinAge, Geographies).
        /// </summary>
        /// <param name="minAge"></param>
        /// <param name="geography"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductsByEligibilityAsync(int? minAge, string geography)
        {
            return (await _repository.GetAllAsync())
                .Where(p =>
                    (!minAge.HasValue || (p.EligibilityCriteria != null && p.EligibilityCriteria.MinAge >= minAge)) &&
                    (string.IsNullOrEmpty(geography) || (p.EligibilityCriteria != null && p.EligibilityCriteria.Geography.Equals(geography, StringComparison.OrdinalIgnoreCase))))
                .ToList();
        }
        /// <summary>
        /// Fetch all products added within the last N days.
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetRecentlyAddedProductsAsync(int days)
        {
            var recentDate = DateTime.UtcNow.AddDays(-days);
            return (await _repository.GetAllAsync())
                .Where(p => p.CreatedDate >= recentDate)
                .ToList();
        }
        /// <summary>
        /// Retrieve all products sorted by their interest rate (ascending or descending).
        /// </summary>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductsSortedByInterestRateAsync(bool ascending = true)
        {
            var products = await _repository.GetAllAsync();

            return ascending
                ? products.OrderBy(p => p.InterestRate.Rate)
                : products.OrderByDescending(p => p.InterestRate.Rate);
        }
        /// <summary>
        /// Count the number of products for each status (e.g., Active, Inactive).
        /// </summary>
        /// <returns></returns>
        public async Task<IDictionary<string, int>> CountProductsByStatusAsync()
        {
            var products = await _repository.GetAllAsync();

            return products.GroupBy(p => p.Status)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count());
        }
        /// <summary>
        /// Fetch all products whose terms and conditions are expiring soon.
        /// </summary>
        /// <param name="expiryThreshold"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductsWithExpiringTermsAsync(DateTime expiryThreshold)
        {
            return (await _repository.GetAllAsync())
                .Where(p => p.TermsAndConditions.Contains("expiry date", StringComparison.OrdinalIgnoreCase) && 
                            DateTime.Parse(p.TermsAndConditions.Split("expiry date: ")[1]) <= expiryThreshold)
                .ToList();
        }
        /// <summary>
        /// Fetch essential details for all products to prepare reports.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<object>> GetProductDetailsForReportAsync()
        {
            return (await _repository.GetAllAsync())
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.ProductType,
                    Currency = p.Currency,
                    p.Status,
                    p.CreatedDate,
                    TotalFees = p.Fees.Sum(f => f.Amount),
                    TotalBenefits = p.Benefits.Count
                })
                .ToList();
        }
        // /// <summary>
        // /// Fetch the top N products based on usage or adoption (assumes UsageCount is tracked in the Product entity).
        // /// </summary>
        // /// <param name="topN"></param>
        // /// <returns></returns>
        // public async Task<IEnumerable<Product>> GetTopProductsByUsageAsync(int topN)
        // {
        //     return (await _repository.GetAllAsync())
        //         .OrderByDescending(p => p.CustomAttributes.ContainsKey("UsageCount")
        //             ? int.Parse(p.CustomAttributes["UsageCount"])
        //             : 0)
        //         .Take(topN)
        //         .ToList();
        // }  
        
        // /// <summary>
        // /// Fetch products by a custom attribute (key-value pair).
        // /// </summary>
        // /// <param name="key"></param>
        // /// <param name="value"></param>
        // /// <returns></returns>
        // public async Task<IEnumerable<Product>> GetProductsByCustomAttributeAsync(string key, string value)
        // {
        //     return (await _repository.GetAllAsync())
        //         .Where(p => p.CustomAttributes.ContainsKey(key) && 
        //                     p.CustomAttributes[key].Equals(value, StringComparison.OrdinalIgnoreCase))
        //         .ToList();
        // } 
    }
}