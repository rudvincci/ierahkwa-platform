using System;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;

namespace Mamey.ApplicationName.Modules.Products.Core.Entities;

/// <summary>
/// Represents a document required for a banking product.
/// </summary>
internal class RequiredDocument
{
    /// <summary>
    /// Primary key for the RequiredDocument entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the required document (e.g., ID Proof, Address Proof).
    /// </summary>
    public string DocumentName { get; set; }

    /// <summary>
    /// Foreign key for the associated Product.
    /// </summary>
    public Guid BankingProductId { get; set; }

    /// <summary>
    /// Navigation property for the associated Product.
    /// </summary>
    public Product Product { get; set; }
}

