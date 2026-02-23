using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.Infrastructure.Services.ERP;

/// <summary>
/// Supplier Service - Manage vendors/suppliers
/// </summary>
public class SupplierService : ISupplierService
{
    private static readonly List<Supplier> _suppliers = [];
    private static readonly Dictionary<Guid, int> _supplierCounters = [];
    private static bool _initialized = false;
    
    public SupplierService()
    {
        if (!_initialized)
        {
            InitializeDefaultSuppliers();
            _initialized = true;
        }
    }
    
    public Task<List<Supplier>> GetAllAsync(Guid companyId)
    {
        var suppliers = _suppliers.Where(s => s.CompanyId == companyId || s.CompanyId == Guid.Empty)
                                  .OrderBy(s => s.Name)
                                  .ToList();
        return Task.FromResult(suppliers);
    }
    
    public Task<Supplier?> GetByIdAsync(Guid id)
    {
        var supplier = _suppliers.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(supplier);
    }
    
    public Task<Supplier> CreateAsync(Supplier supplier)
    {
        supplier.Id = Guid.NewGuid();
        supplier.CreatedAt = DateTime.UtcNow;
        
        if (string.IsNullOrEmpty(supplier.Code))
        {
            if (!_supplierCounters.ContainsKey(supplier.CompanyId))
                _supplierCounters[supplier.CompanyId] = 1000;
            _supplierCounters[supplier.CompanyId]++;
            supplier.Code = $"SUP-{_supplierCounters[supplier.CompanyId]:D4}";
        }
        
        _suppliers.Add(supplier);
        return Task.FromResult(supplier);
    }
    
    public Task<Supplier> UpdateAsync(Supplier supplier)
    {
        var index = _suppliers.FindIndex(s => s.Id == supplier.Id);
        if (index >= 0)
        {
            supplier.UpdatedAt = DateTime.UtcNow;
            _suppliers[index] = supplier;
        }
        return Task.FromResult(supplier);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var supplier = _suppliers.FirstOrDefault(s => s.Id == id);
        if (supplier != null)
        {
            _suppliers.Remove(supplier);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    private void InitializeDefaultSuppliers()
    {
        var defaultSuppliers = new List<Supplier>
        {
            new()
            {
                Code = "SUP-0001",
                Name = "Tech Components Inc.",
                LegalName = "Tech Components International Inc.",
                TaxId = "TC-123456789",
                ContactName = "John Smith",
                Email = "orders@techcomponents.com",
                Phone = "+1 555-0201",
                Address = "123 Tech Park Drive",
                City = "San Jose",
                State = "CA",
                Country = "USA",
                PaymentTermDays = 30,
                Currency = "USD",
                IsActive = true,
                TotalPurchases = 125000
            },
            new()
            {
                Code = "SUP-0002",
                Name = "Global Logistics SA",
                LegalName = "Global Logistics Services SA de CV",
                TaxId = "GLS-987654321",
                ContactName = "María García",
                Email = "ventas@globallogistics.mx",
                Phone = "+52 55 1234 5678",
                Address = "Av. Industrial 456",
                City = "Ciudad de México",
                State = "CDMX",
                Country = "Mexico",
                PaymentTermDays = 15,
                Currency = "MXN",
                IsActive = true,
                TotalPurchases = 85000
            },
            new()
            {
                Code = "SUP-0003",
                Name = "Cloud Services Ltd",
                LegalName = "Cloud Services Limited",
                TaxId = "CSL-456789123",
                ContactName = "James Wilson",
                Email = "billing@cloudservices.io",
                Phone = "+44 20 7946 0958",
                Address = "10 Cloud Street",
                City = "London",
                Country = "UK",
                PaymentTermDays = 45,
                Currency = "GBP",
                IsActive = true,
                TotalPurchases = 45000
            }
        };
        
        _suppliers.AddRange(defaultSuppliers);
    }
}
