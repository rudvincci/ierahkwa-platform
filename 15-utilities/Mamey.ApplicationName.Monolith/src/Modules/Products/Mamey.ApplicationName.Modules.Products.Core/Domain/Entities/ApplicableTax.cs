using System;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;

namespace Mamey.ApplicationName.Modules.Products.Core.Entities;

/// <summary>
/// Represents a tax applicable to a banking product.
/// </summary>
internal class ApplicableTax
{
    /// <summary>
    /// Primary key for the ApplicableTax entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the tax (e.g., VAT, GST).
    /// </summary>
    public string TaxName { get; set; }

    /// <summary>
    /// Tax rate applicable to the banking product.
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Foreign key for the associated Product.
    /// </summary>
    public Guid BankingProductId { get; set; }

    /// <summary>
    /// Navigation property for the associated Product.
    /// </summary>
    public Product Product { get; set; }
}
