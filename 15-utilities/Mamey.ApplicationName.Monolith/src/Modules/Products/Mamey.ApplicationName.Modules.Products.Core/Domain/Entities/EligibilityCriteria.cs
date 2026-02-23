using System;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;

namespace Mamey.ApplicationName.Modules.Products.Core.Entities;

/// <summary>
/// Represents eligibility criteria for a banking product.
/// </summary>
internal class EligibilityCriteria
{
    /// <summary>
    /// Primary key for the EligibilityCriteria entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Minimum age required.
    /// </summary>
    public int? MinAge { get; set; }

    /// <summary>
    /// Maximum age allowed.
    /// </summary>
    public int? MaxAge { get; set; }

    /// <summary>
    /// Minimum income required.
    /// </summary>
    public decimal? MinimumIncome { get; set; }

    /// <summary>
    /// Allowed geography (e.g., country or region).
    /// </summary>
    public string? Geography { get; set; }

    /// <summary>
    /// Other criteria in a descriptive format.
    /// </summary>
    public string? OtherCriteria { get; set; }

    /// <summary>
    /// Foreign key for the associated Product.
    /// </summary>
    public Guid BankingProductId { get; set; }

    /// <summary>
    /// Navigation property for the associated Product.
    /// </summary>
    public Product Product { get; set; }

}
