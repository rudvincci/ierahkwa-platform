using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;
using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Mamey.ApplicationName.Modules.Products.Core.Repositories;

namespace Mamey.ApplicationName.Modules.Products.Core.EF.Repositories;

internal class BankingProductRepository : IBankingProductRepository
{
    private readonly BankingProductDbContext _context;

    public BankingProductRepository(BankingProductDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(b=> b.InterestRate)
            .Include(b=> b.Limits)
            .Include(b => b.Fees)
            .Include(b => b.Benefits)
            .Include(b => b.ApplicableTaxes)
            .Include(b => b.RequiredDocuments)
            .Include(b => b.EligibilityCriteria)
            .ToListAsync();
    }
    
    
    public async Task<Product> GetByIdAsync(Guid productId)
    {
        return await _context.Products
            .Where(p => !p.IsDeleted)
            .Include(b=> b.InterestRate)
            .Include(b=> b.Limits)
            .Include(b => b.Fees)
            .Include(b => b.Benefits)
            .Include(b => b.ApplicableTaxes)
            .Include(b => b.RequiredDocuments)
            .Include(b => b.EligibilityCriteria)
            .FirstOrDefaultAsync();
    }

    public async Task<Product> GetByNameAsync(string name)
    {
        return await _context.Products
            .Where(p => p.Name.ToLower() == name.ToLower() && !p.IsDeleted)
            .Include(b=> b.InterestRate)
            .Include(b=> b.Limits)
            .Include(b => b.Fees)
            .Include(b => b.Benefits)
            .Include(b => b.ApplicableTaxes)
            .Include(b => b.RequiredDocuments)
            .Include(b => b.EligibilityCriteria)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Benefit>> GetBenefitsByProductIdAsync(Guid productId)
    {
        return await _context.Benefits
            .Where(b => b.BankingProductId == productId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Fee>> GetFeesByProductIdAsync(Guid productId)
    {
        return await _context.Fees
            .Where(f => f.Id == productId)
            .ToListAsync();
    }
}
