using System;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;

namespace Mamey.ApplicationName.Modules.Products.Core.Entities;

/// <summary>
/// Represents a benefit associated with a banking product.
/// </summary>
internal class Benefit
{
    /// <summary>
    /// Primary key for the Benefit entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Type of the benefit (e.g., Cashback, Points, Insurance).
    /// </summary>
    public string BenefitType { get; set; }

    /// <summary>
    /// Description of the benefit.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Foreign key for the associated Product.
    /// </summary>
    public Guid BankingProductId { get; set; }

    /// <summary>
    /// Navigation property for the associated Product.
    /// </summary>
    public Product Product { get; set; }
}
