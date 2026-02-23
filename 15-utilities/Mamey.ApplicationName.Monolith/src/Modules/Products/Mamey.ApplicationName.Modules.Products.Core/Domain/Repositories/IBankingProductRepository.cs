using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;
using Mamey.ApplicationName.Modules.Products.Core.Entities;

namespace Mamey.ApplicationName.Modules.Products.Core.Repositories;
internal interface IBankingProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> GetByIdAsync(Guid productId);
    Task<Product> GetByNameAsync(string name);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<IEnumerable<Benefit>> GetBenefitsByProductIdAsync(Guid productId);
    Task<IEnumerable<Fee>> GetFeesByProductIdAsync(Guid productId);
}
