using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.Infrastructure.Services.ERP
{
    public class ProductService : IProductService
    {
        private static readonly List<Product> _products = new();
        private static readonly List<ProductCategory> _categories = new();
        private static int _productCounter = 1000;
        
        public Task<List<Product>> GetAllAsync(Guid companyId)
        {
            var products = _products.Where(p => p.CompanyId == companyId)
                                    .OrderBy(p => p.Name)
                                    .ToList();
            return Task.FromResult(products);
        }
        
        public Task<Product?> GetByIdAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }
        
        public Task<Product?> GetBySKUAsync(Guid companyId, string sku)
        {
            var product = _products.FirstOrDefault(p => 
                p.CompanyId == companyId && 
                p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(product);
        }
        
        public Task<Product> CreateAsync(Product product)
        {
            product.Id = Guid.NewGuid();
            if (string.IsNullOrEmpty(product.SKU))
            {
                product.SKU = $"PROD-{++_productCounter:D5}";
            }
            product.CreatedAt = DateTime.UtcNow;
            _products.Add(product);
            return Task.FromResult(product);
        }
        
        public Task<Product> UpdateAsync(Product product)
        {
            var index = _products.FindIndex(p => p.Id == product.Id);
            if (index >= 0)
            {
                _products[index] = product;
            }
            return Task.FromResult(product);
        }
        
        public Task<bool> DeleteAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        
        public Task<List<Product>> SearchAsync(Guid companyId, string searchTerm)
        {
            var term = searchTerm.ToLower();
            var products = _products.Where(p => 
                p.CompanyId == companyId &&
                (p.Name.ToLower().Contains(term) ||
                 p.SKU.ToLower().Contains(term) ||
                 p.Barcode.Contains(term) ||
                 p.Description.ToLower().Contains(term)))
                .OrderBy(p => p.Name)
                .ToList();
            return Task.FromResult(products);
        }
        
        public Task<List<Product>> GetLowStockAsync(Guid companyId)
        {
            var products = _products.Where(p => 
                p.CompanyId == companyId &&
                p.TrackInventory &&
                p.StockQuantity <= p.ReorderLevel)
                .OrderBy(p => p.StockQuantity)
                .ToList();
            return Task.FromResult(products);
        }
        
        public Task<List<ProductCategory>> GetCategoriesAsync(Guid companyId)
        {
            var categories = _categories.Where(c => c.CompanyId == companyId)
                                        .OrderBy(c => c.Name)
                                        .ToList();
            return Task.FromResult(categories);
        }
    }
}
