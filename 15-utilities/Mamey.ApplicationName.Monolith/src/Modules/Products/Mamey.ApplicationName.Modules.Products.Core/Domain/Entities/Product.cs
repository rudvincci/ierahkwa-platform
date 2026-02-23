using System;
using System.Collections.Generic;
using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Mamey.Bank.Shared.Types;
using Mamey.Types;
using AccountCategory = Mamey.Bank.Shared.Types.AccountCategory;

namespace Mamey.Bank.Modules.BankingProducts.Core.Entities;

/// <summary>
/// Represents the top-level (parent) product family 
/// (e.g., "Standard Checking", "High-Yield Savings", etc.).
/// Variants (tiers) of this product are stored as ProductVariant.
/// </summary>
internal class Product
{
   
    public Guid Id { get; set; }
    /// <summary>
    /// Broad account type (e.g., CheckingAccount, SavingsAccount, etc.).
    /// </summary>
    public AccountType ProductType { get; set; }
    /// <summary>
    /// Base name of the product (e.g., "Standard Checking").
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// High-level description for this product family.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Default or primary currency for this product family (e.g., USD, EUR).
    /// </summary>
    public Currency Currency { get; set; } = new Currency("EUR");
    /// <summary>
    /// Global flag indicating if the product is marked as deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
    /// <summary>
    /// Versioning for concurrency or product lifecycle tracking.
    /// </summary>
    public int Version { get; set; } = 0;
    /// <summary>
    /// High-level terms and conditions for all variants under this product family.
    /// Variants can override or extend these.
    /// </summary>
    public string TermsAndConditions { get; set; } = string.Empty;
    /// <summary>
    /// Possible statuses: Active, Inactive, Discontinued, etc.
    /// </summary>
    public ProductStatus Status { get; set; } = ProductStatus.Active;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public Guid? InterestRateId { get; set; }
    public InterestRate? InterestRate { get; set; } = null;
    public Limits Limits { get; set; } = new Limits();
    public Guid LimitsId { get; set; }
    public EligibilityCriteria EligibilityCriteria { get; set; }
    public ICollection<Fee> Fees { get; set; } = new List<Fee>();
    public ICollection<Benefit> Benefits { get; set; } = new List<Benefit>();
    public AccountCategory AccountCategory { get; set; }
    public ICollection<RequiredDocument> RequiredDocuments { get; set; } = new List<RequiredDocument>();
    public ICollection<ApplicableTax> ApplicableTaxes { get; set; } = new List<ApplicableTax>();
}

internal enum ProductStatus : byte
{
    Active = 1,
    Inactive = 2,
    Discontinued = 3
}