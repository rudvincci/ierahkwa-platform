using System;

namespace Mamey.ApplicationName.Modules.Products.Core.DTO;

public class BankingProductSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ProductType { get; set; }
    public string AccountCategory { get; set; }
    public string Status { get; set; }
}
