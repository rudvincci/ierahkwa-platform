using System;
using System.Collections.Generic;
using Mamey.CQRS.Commands;

namespace Mamey.ApplicationName.Modules.Products.Core.Commands;

public class CreateBankingProduct : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<string> Currency { get; set; }
    public string ProductType { get; set; }
    public string AccountCategory { get; set; }
    public double InterestRate { get; set; }
    public string InterestRateType { get; set; }
    public string Status { get; set; }
}

public record DeleteBankingProduct(Guid Id) : ICommand;

public record CreateBulkProducts(IEnumerable<CreateBankingProduct> Products) : ICommand;