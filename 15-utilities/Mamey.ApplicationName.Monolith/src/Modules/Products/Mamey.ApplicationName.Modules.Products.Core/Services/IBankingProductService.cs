using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;
using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Mamey.Bank.Shared.Types;
using AccountCategory = Mamey.Bank.Shared.Types.AccountCategory;

namespace Mamey.ApplicationName.Modules.Products.Core.Services
{
    internal interface IBankingProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(Guid productId);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid productId);
        Task<IEnumerable<Benefit>> GetBenefitsByProductIdAsync(Guid productId);
        Task<IEnumerable<Fee>> GetFeesByProductIdAsync(Guid productId);
        Task<IEnumerable<Product>> SearchProductsAsync(
            AccountType? productType = null,
            string currency = null,
            AccountCategory? accountCategory = null);
        Task AddProductsAsync(IEnumerable<Product> products);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<Product> GetProductByNameAsync(string name);
    }
}