using System;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;

namespace Mamey.ApplicationName.Modules.Products.Core.Entities;

internal class Limits
{
    public Guid Id { get; set; }
    
    public Guid BankingProductId { get; set; }
    public decimal? MinimumBalance { get; set; }
    public decimal? MaximumBalance { get; set; }
    public decimal? DailyTransactionLimit { get; set; }
    public decimal? WithdrawalLimit { get; set; }
    
    public Product Product { get; set; }
}
