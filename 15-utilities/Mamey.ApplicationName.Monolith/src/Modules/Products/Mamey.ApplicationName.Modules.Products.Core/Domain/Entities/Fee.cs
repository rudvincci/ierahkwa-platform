using System;
using System.ComponentModel.DataAnnotations;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;

namespace Mamey.ApplicationName.Modules.Products.Core.Entities;

internal class Fee
{
    public Guid Id { get; set; }
    public Guid BankingProductId { get; set; }
    public FeeType FeeType { get; set; }
    public decimal Amount { get; set; }
    public FeeFrequency Frequency { get; set; }
    public Product Product { get; set; }
}

// public class Currency
// {
//     [MaxLength(3)]
//     public string Code { get; set; }
// }