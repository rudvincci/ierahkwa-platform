using System;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;


namespace Mamey.ApplicationName.Modules.Products.Core.Entities;

internal class InterestRate
{
    public Guid Id { get; set; }
    public Guid BankingProductId { get; set; }
    public double Rate { get; set; }
    public InterestRateType Type { get; set; }
    public string CompoundingFrequency { get; set; }
    
    public Product? Product { get; set; }
}

